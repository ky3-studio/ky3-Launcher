//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Annotation.HttpClient;
using Launcher.Web.Endpoint.Hoyolab;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using Launcher.Web.Response;
using System.Net.Http;

namespace Launcher.Web.Hoyolab.Bbs.User;

[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class UserClientOversea : IUserClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial UserClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<UserFullInfoWrapper>> GetUserFullInfoAsync(Model.Entity.User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.UserFullInfoQuery(user.Aid))
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        Response<UserFullInfoWrapper>? resp = await builder
            .SendAsync<Response<UserFullInfoWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}