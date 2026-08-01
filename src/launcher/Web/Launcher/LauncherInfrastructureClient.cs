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

    public async ValueTask<LauncherResponse<LauncherPackageInformation>> GetLauncherVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PatchSnapLauncher())
            .Get();

        LauncherResponse<LauncherPackageInformation>? resp = await builder.SendAsync<LauncherResponse<LauncherPackageInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}
