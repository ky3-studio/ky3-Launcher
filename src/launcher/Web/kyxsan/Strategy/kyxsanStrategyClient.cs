//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Model.Primitive;
using kyxsan.Service.kyxsan;
using kyxsan.Web.Endpoint.kyxsan;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Collections.Immutable;
using System.Net.Http;

namespace kyxsan.Web.kyxsan.Strategy;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanStrategyClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly kyxsanUserOptions kyxsanUserOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanStrategyClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<kyxsanResponse<ImmutableDictionary<AvatarId, Strategy>>> GetStrategyAllAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StrategyAll())
            .Get();

        kyxsanResponse<ImmutableDictionary<AvatarId, Strategy>>? resp = await builder.SendAsync<kyxsanResponse<ImmutableDictionary<AvatarId, Strategy>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<ImmutableDictionary<AvatarId, Strategy>>> GetStrategyItemAsync(AvatarId avatarId, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StrategyItem(avatarId))
            .Get();

        kyxsanResponse<ImmutableDictionary<AvatarId, Strategy>>? resp = await builder.SendAsync<kyxsanResponse<ImmutableDictionary<AvatarId, Strategy>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}