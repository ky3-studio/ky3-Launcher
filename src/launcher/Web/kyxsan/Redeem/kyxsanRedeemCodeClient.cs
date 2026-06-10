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
using System.Net.Http;

namespace kyxsan.Web.kyxsan.Redeem;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanRedeemCodeClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanRedeemCodeClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<kyxsanResponse<RedeemUseResult>> UseRedeemCodeAsync(string? accessToken, RedeemUseRequest request, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RedeemCodeUse())
            .SetAccessToken(accessToken)
            .PostJson(request);

        kyxsanResponse<RedeemUseResult>? resp = await builder
            .SendAsync<kyxsanResponse<RedeemUseResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}