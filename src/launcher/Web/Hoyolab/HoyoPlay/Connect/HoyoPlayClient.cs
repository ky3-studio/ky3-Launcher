//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Service.Game.Scheme;
using kyxsan.Web.Endpoint.Hoyolab;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Package;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.Hoyolab.HoyoPlay.Connect;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HoyoPlayClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HoyoPlayClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<GamePackagesWrapper>> GetPackagesAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectGamePackages(scheme))
            .Get();

        Response<GamePackagesWrapper>? resp = await builder
            .SendAsync<Response<GamePackagesWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameChannelSDKsWrapper>> GetChannelSDKAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectGameChannelSDKs(scheme))
            .Get();

        Response<GameChannelSDKsWrapper>? resp = await builder
            .SendAsync<Response<GameChannelSDKsWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<DeprecatedFileConfigurationsWrapper>> GetDeprecatedFileConfigurationsAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectDeprecatedFileConfigs(scheme))
            .Get();

        Response<DeprecatedFileConfigurationsWrapper>? resp = await builder
            .SendAsync<Response<DeprecatedFileConfigurationsWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameBranchesWrapper>> GetBranchesAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectGameBranches(scheme))
            .Get();

        Response<GameBranchesWrapper>? resp = await builder
            .SendAsync<Response<GameBranchesWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}