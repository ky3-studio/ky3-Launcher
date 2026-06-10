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
using kyxsan.Web.Endpoint.Hoyolab;
using kyxsan.Web.Hoyolab.Takumi.Auth;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.Hoyolab.Takumi.Binding;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class BindingClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial BindingClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesOverseaAwareAsync(User user, CancellationToken token = default)
    {
        if (user.IsOversea)
        {
            return await GetUserGameRolesByCookieAsync(user, token).ConfigureAwait(false);
        }

        Response<ActionTicketWrapper> actionTicketResponse = await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetActionTicketBySTokenAsync("game_role", user, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(actionTicketResponse, serviceProvider, out ActionTicketWrapper? wrapper))
        {
            string actionTicket = wrapper.Ticket;
            return await GetUserGameRolesByActionTicketAsync(actionTicket, user, token).ConfigureAwait(false);
        }

        return Response.Response.CloneReturnCodeAndMessage<ListWrapper<UserGameRole>, ActionTicketWrapper>(actionTicketResponse);
    }

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByActionTicketAsync(string actionTicket, User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(user.IsOversea).UserGameRolesByActionTicket(actionTicket))
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .SendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByCookieAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(user.IsOversea).UserGameRolesByCookie())
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .SendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}