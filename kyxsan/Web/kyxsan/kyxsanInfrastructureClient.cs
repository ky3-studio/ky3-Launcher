//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Web.Endpoint.kyxsan;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Win32;
using System.Collections.Immutable;
using System.Net.Http;

namespace kyxsan.Web.kyxsan;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanInfrastructureClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanInfrastructureClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<kyxsanResponse<IPInformation>> GetIPInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().Ip())
            .Get();

        kyxsanResponse<IPInformation>? resp = await builder.SendAsync<kyxsanResponse<IPInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<kyxsanPackageInformation>> GetkyxsanVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().PatchSnapkyxsan())
            .Get();

        kyxsanResponse<kyxsanPackageInformation>? resp = await builder.SendAsync<kyxsanResponse<kyxsanPackageInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse> AmIBannedAsync(string uid, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().AmIBanned())
            .SetHeader("x-kyxsan-island-identifier", kyxsanNative.Instance.ExchangeGameUidForIdentifier1820(uid))
            .Get();

        kyxsanResponse? resp = await builder.SendAsync<kyxsanResponse>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<ImmutableArray<GitRepository>>> GetGitRepositoryAsync(string name, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().GitRepository(name))
            .Get();

        kyxsanResponse<ImmutableArray<GitRepository>>? resp = await builder.SendAsync<kyxsanResponse<ImmutableArray<GitRepository>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}