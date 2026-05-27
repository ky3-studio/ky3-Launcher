//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Core.LifeCycle;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.Behavior.Action;
using kyxsan.UI.Xaml.View.Window.WebView2;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;
using kyxsan.Web.Response;

namespace kyxsan.Service.SignIn;

[Service(ServiceLifetime.Singleton, typeof(ISignInService))]
internal sealed partial class SignInService : ISignInService
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial SignInService(IServiceProvider serviceProvider);

    public async ValueTask<bool> ClaimSignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.SignAsync(userAndUid, token).ConfigureAwait(false);
            if (ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? result))
            {
                return true;
            }

            string message = resultResponse.Message;

            if (resultResponse.ReturnCode is (int)KnownReturnCode.AlreadySignedIn)
            {
                return true;
            }

            if (string.IsNullOrEmpty(message))
            {
                message = $"RiskCode: {result?.RiskCode}";
            }

            messenger.Send(InfoBarMessage.Error(SH.FormatServiceSignInClaimRewardFailed(message)));
            await FallbackToWebView2SignInAsync().ConfigureAwait(false);
            return false;
        }
    }

    public async ValueTask<bool> ClaimResignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.ReSignAsync(userAndUid, token).ConfigureAwait(false);
            if (ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? signInResult))
            {
                return true;
            }

            string message = resultResponse.Message;

            if (resultResponse.ReturnCode is (int)KnownReturnCode.ResignQuotaUsedUp or (int)KnownReturnCode.PleaseSignInFirst or (int)KnownReturnCode.NoAvailableResignDate)
            {
                messenger.Send(InfoBarMessage.Error(message));
                return false;
            }

            if (resultResponse.ReturnCode is (int)KnownReturnCode.NotEnoughCoin)
            {
                message = SH.ViewModelSignInReSignInNotEnoughCoinMessage;
                messenger.Send(InfoBarMessage.Error(message));
                return false;
            }

            if (string.IsNullOrEmpty(message))
            {
                message = $"RiskCode: {signInResult?.RiskCode}";
            }

            messenger.Send(InfoBarMessage.Error(SH.FormatServiceReSignInClaimRewardFailed(message)));
            await FallbackToWebView2SignInAsync().ConfigureAwait(false);
            return false;
        }
    }

    private async ValueTask FallbackToWebView2SignInAsync()
    {
        await taskContext.SwitchToMainThreadAsync();

        if (currentXamlWindowReference.XamlRoot is not { } xamlRoot)
        {
            return;
        }

        MiHoYoJSBridgeWebView2ContentProvider provider = new()
        {
            SourceProvider = new SignInJSBridgeUriSourceProvider(),
        };

        ShowWebView2WindowAction.Show(provider, xamlRoot);
    }
}