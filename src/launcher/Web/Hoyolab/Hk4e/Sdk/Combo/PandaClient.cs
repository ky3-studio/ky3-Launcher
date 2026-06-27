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

namespace Launcher.Web.Hoyolab.Hk4e.Sdk.Combo;

[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PandaClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial PandaClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<UrlWrapper>> QRCodeFetchAsync(CancellationToken token = default)
    {
        GameLoginRequest options = GameLoginRequest.Create(8, HoyolabOptions.DeviceId40);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.QrCodeFetch())
            .PostJson(options);

        Response<UrlWrapper>? resp = await builder
            .SendAsync<Response<UrlWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameLoginResult>> QRCodeQueryAsync(string ticket, CancellationToken token = default)
    {
        GameLoginRequest options = GameLoginRequest.Create(8, HoyolabOptions.DeviceId40, ticket);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.QrCodeQuery())
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId40)
            .PostJson(options);

        Response<GameLoginResult>? resp = await builder
            .SendAsync<Response<GameLoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}