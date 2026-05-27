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
using kyxsan.Web.kyxsan.Redeem;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace kyxsan.Web.kyxsan.kyxsanAsAService;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanAsAServiceClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanAsAServiceClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<kyxsanResponse> UploadAnnouncementAsync(string? accessToken, UploadAnnouncement uploadAnnouncement, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().AnnouncementUpload())
            .SetAccessToken(accessToken)
            .PostJson(uploadAnnouncement);

        kyxsanResponse? resp = await builder
            .SendAsync<kyxsanResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse> CdnCompensationAsync(string? accessToken, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().CdnCompensation(days))
            .SetAccessToken(accessToken)
            .Get();

        kyxsanResponse? resp = await builder
            .SendAsync<kyxsanResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse> CdnDesignationAsync(string? accessToken, string userName, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().CdnDesignation(userName, days))
            .SetAccessToken(accessToken)
            .Get();

        kyxsanResponse? resp = await builder
            .SendAsync<kyxsanResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<RedeemGenerateResult>> GenerateRedeemCodesAsync(string? accessToken, RedeemGenerateRequest request, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RedeemCodeGenerate())
            .SetAccessToken(accessToken)
            .PostJson(request);

        kyxsanResponse<RedeemGenerateResult>? resp = await builder
            .SendAsync<kyxsanResponse<RedeemGenerateResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}