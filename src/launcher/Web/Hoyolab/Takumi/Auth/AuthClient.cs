//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Annotation.HttpClient;
using Launcher.Model.Entity;
using Launcher.Web.Endpoint.Hoyolab;
using Launcher.Web.Hoyolab.DataSigning;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using Launcher.Web.Response;
using System.Net.Http;

namespace Launcher.Web.Hoyolab.Takumi.Auth;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class AuthClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial AuthClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<ActionTicketWrapper>> GetActionTicketBySTokenAsync(string action, User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        string stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN) ?? string.Empty;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(user.IsOversea).AuthActionTicket(action, stoken, user.Aid))
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.K2, true).ConfigureAwait(false);

        Response<ActionTicketWrapper>? resp = await builder
            .SendAsync<Response<ActionTicketWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<NameToken>>> GetMultiTokenByLoginTicketAsync(Cookie cookie, bool isOversea, CancellationToken token = default)
    {
        Response<ListWrapper<NameToken>>? resp = null;
        if (cookie.TryGetLoginTicket(out Cookie? loginTicketCookie))
        {
            string loginTicket = loginTicketCookie[Cookie.LOGIN_TICKET];
            string loginUid = loginTicketCookie[Cookie.LOGIN_UID];

            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetRequestUri(apiEndpointsFactory.Create(isOversea).AuthMultiToken(loginTicket, loginUid))
                .Get();

            resp = await builder
                .SendAsync<Response<ListWrapper<NameToken>>>(httpClient, token)
                .ConfigureAwait(false);
        }

        return Response.Response.DefaultIfNull(resp);
    }
}