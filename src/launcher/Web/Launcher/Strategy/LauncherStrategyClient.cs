//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Annotation.HttpClient;
using Launcher.Model.Primitive;
using Launcher.Service.Launcher;
using Launcher.Web.Endpoint.Launcher;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using System.Collections.Immutable;
using System.Net.Http;

namespace Launcher.Web.Launcher.Strategy;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class LauncherStrategyClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly LauncherUserOptions LauncherUserOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial LauncherStrategyClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<LauncherResponse<ImmutableDictionary<AvatarId, Strategy>>> GetStrategyAllAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StrategyAll())
            .Get();

        LauncherResponse<ImmutableDictionary<AvatarId, Strategy>>? resp = await builder.SendAsync<LauncherResponse<ImmutableDictionary<AvatarId, Strategy>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<ImmutableDictionary<AvatarId, Strategy>>> GetStrategyItemAsync(AvatarId avatarId, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StrategyItem(avatarId))
            .Get();

        LauncherResponse<ImmutableDictionary<AvatarId, Strategy>>? resp = await builder.SendAsync<LauncherResponse<ImmutableDictionary<AvatarId, Strategy>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}