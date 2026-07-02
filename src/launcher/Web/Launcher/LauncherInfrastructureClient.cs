//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Annotation.HttpClient;
using Launcher.Web.Endpoint.Launcher;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using Launcher.Win32;
using System.Collections.Immutable;
using System.Net.Http;

namespace Launcher.Web.Launcher;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class LauncherInfrastructureClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial LauncherInfrastructureClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<LauncherResponse<IPInformation>> GetIPInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().Ip())
            .Get();

        LauncherResponse<IPInformation>? resp = await builder.SendAsync<LauncherResponse<IPInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<LauncherPackageInformation>> GetLauncherVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PatchSnapLauncher())
            .Get();

        LauncherResponse<LauncherPackageInformation>? resp = await builder.SendAsync<LauncherResponse<LauncherPackageInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse> AmIBannedAsync(string uid, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().AmIBanned())
            .SetHeader("x-Launcher-island-identifier", LauncherNative.Instance.ExchangeGameUidForIdentifier1820(uid))
            .Get();

        LauncherResponse? resp = await builder.SendAsync<LauncherResponse>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<ImmutableArray<GitRepository>>> GetGitRepositoryAsync(string name, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().GitRepository(name))
            .Get();

        LauncherResponse<ImmutableArray<GitRepository>>? resp = await builder.SendAsync<LauncherResponse<ImmutableArray<GitRepository>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}
