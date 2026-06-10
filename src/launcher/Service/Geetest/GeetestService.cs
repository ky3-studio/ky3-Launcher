//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.LifeCycle;
using kyxsan.UI.Xaml.Behavior.Action;
using kyxsan.UI.Xaml.View.Window.WebView2;
using kyxsan.Web.Hoyolab.Passport;
using kyxsan.Web.Hoyolab.Takumi.GameRecord;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Verification;
using kyxsan.Web.kyxsan.Geetest;
using kyxsan.Web.Response;

namespace kyxsan.Service.Geetest;

[Service(ServiceLifetime.Transient, typeof(IGeetestService))]
internal sealed partial class GeetestService : IGeetestService
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly CustomGeetestClient customGeetestClient;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly ITaskContext taskContext;
    private readonly CardClient cardClient;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial GeetestService(IServiceProvider serviceProvider);

    public async ValueTask<GeetestData?> TryVerifyGtChallengeAsync(string gt, string challenge, bool isOversea, CancellationToken token = default)
    {
        GeetestResponse response = await customGeetestClient.VerifyAsync(gt, challenge, token).ConfigureAwait(false);

        if (response is { Code: 0, Data: { } data })
        {
            return data;
        }

        string? result = await PrivateVerifyByWebViewAsync(gt, challenge, isOversea, token).ConfigureAwait(false);
        if (string.IsNullOrEmpty(result))
        {
            return default;
        }

        GeetestWebResponse? webResponse = JsonSerializer.Deserialize<GeetestWebResponse>(result, jsonOptions);
        ArgumentNullException.ThrowIfNull(webResponse);

        return new()
        {
            Gt = gt,
            Challenge = webResponse.Challenge,
            Validate = webResponse.Validate,
        };
    }

    public async ValueTask<string?> TryVerifyXrpcChallengeAsync(Model.Entity.User user, CardVerifiationHeaders headers, CancellationToken token = default)
    {
        Response<GeetestVerification> registrationResponse = await cardClient.CreateVerificationAsync(user, headers, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(registrationResponse, messenger, out GeetestVerification? registration))
        {
            return default;
        }

        if (await TryVerifyGtChallengeAsync(registration.Gt, registration.Challenge, user.IsOversea, token).ConfigureAwait(false) is not { } data)
        {
            return default;
        }

        Response<VerificationResult> verifyResponse = await cardClient.VerifyVerificationAsync(user, headers, data.Challenge, data.Validate, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(verifyResponse, messenger, out VerificationResult? result))
        {
            return default;
        }

        return result.Challenge;
    }

    public async ValueTask<bool> TryVerifyAigisSessionAsync(IAigisProvider provider, string? rawSession, bool isOversea, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(rawSession))
        {
            return false;
        }

        AigisSession? session = JsonSerializer.Deserialize<AigisSession>(rawSession, jsonOptions);
        ArgumentNullException.ThrowIfNull(session);

        GeetestVerification? verification = JsonSerializer.Deserialize<GeetestVerification>(session.Data, jsonOptions);
        ArgumentNullException.ThrowIfNull(verification);

        if (await TryVerifyGtChallengeAsync(verification.Gt, verification.Challenge, isOversea, token).ConfigureAwait(false) is not { } data)
        {
            // Custom Geetest failed and user closed the window without completing the verification
            return false;
        }

        GeetestWebResponse result = new(data.Challenge, data.Validate);

        provider.Aigis = $"{session.SessionId};{Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(result))}";
        return true;
    }

    private async ValueTask<string?> PrivateVerifyByWebViewAsync(string gt, string challenge, bool isOversea, CancellationToken token)
    {
        if (XamlApplicationLifetime.Exiting)
        {
            return default;
        }

        await taskContext.SwitchToMainThreadAsync();
        token.ThrowIfCancellationRequested();
        if (currentXamlWindowReference.XamlRoot is { } xamlRoot)
        {
            GeetestWebView2ContentProvider contentProvider = new(gt, challenge, isOversea);
            ShowWebView2WindowAction.Show(contentProvider, xamlRoot);

            await taskContext.SwitchToBackgroundAsync();
            token.ThrowIfCancellationRequested();
            return await contentProvider.GetResultAsync().ConfigureAwait(false);
        }

        return default!;
    }
}