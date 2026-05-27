//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Service;
using kyxsan.ViewModel.User;
using kyxsan.Web.Endpoint.Hoyolab;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class SignInClientOversea : ISignInClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly CultureOptions cultureOptions;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial SignInClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<SignInRewardInfo>> GetInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.LunaSolInfo(userAndUid.Uid, cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .Get();

        Response<SignInRewardInfo>? resp = await builder
            .SendAsync<Response<SignInRewardInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.LunaSolHome(cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(user, CookieType.CookieToken)
            .Get();

        Response<Reward>? resp = await builder
            .SendAsync<Response<Reward>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.LunaSolSign())
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .PostJson(new SignInData(apiEndpoints, userAndUid.Uid));

        Response<SignInResult>? resp = await builder
            .SendAsync<Response<SignInResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInRewardReSignInfo>> GetResignInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.LunaSolResignInfo(userAndUid.Uid, cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .Get();

        Response<SignInRewardReSignInfo>? resp = await builder
            .SendAsync<Response<SignInRewardReSignInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> ReSignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.LunaSolReSign(cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .PostJson(new SignInData(apiEndpoints, userAndUid.Uid));

        Response<SignInResult>? resp = await builder
            .SendAsync<Response<SignInResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}