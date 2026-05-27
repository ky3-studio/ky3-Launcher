//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Model.Entity;
using kyxsan.Service.Geetest;
using kyxsan.Web.Endpoint.Hoyolab;
using kyxsan.Web.Hoyolab.DataSigning;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Verification;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord;

[HttpClient(HttpClientConfiguration.XRpc)]
internal sealed partial class CardClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial CardClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<GeetestVerification>> CreateVerificationAsync(User user, CardVerifiationHeaders headers, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CardCreateVerification(true))
            .SetUserCookieAndFpHeader(user, CookieType.Cookie)
            .SetHeader("x-rpc-challenge_game", $"{headers.ChallengeGame}")
            .SetHeader("x-rpc-challenge_path", headers.ChallengePath)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<GeetestVerification>? resp = await builder
            .SendAsync<Response<GeetestVerification>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<VerificationResult>> VerifyVerificationAsync(User user, CardVerifiationHeaders headers, string challenge, string validate, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CardVerifyVerification())
            .SetUserCookieAndFpHeader(user, CookieType.Cookie)
            .SetHeader("x-rpc-challenge_game", $"{headers.ChallengeGame}")
            .SetHeader("x-rpc-challenge_path", headers.ChallengePath)
            .PostJson(new GeetestWebResponse(challenge, validate));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<VerificationResult>? resp = await builder
            .SendAsync<Response<VerificationResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<DailyNote.WidgetDailyNote>> GetWidgetDataAsync(User user, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CardWidgetData2())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X6, false).ConfigureAwait(false);

        // Suppress HTTP error logging since this is used as a silent fallback for 5003/1034
        Response<DailyNote.WidgetDailyNote>? resp = await builder
            .SendAsync<Response<DailyNote.WidgetDailyNote>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}