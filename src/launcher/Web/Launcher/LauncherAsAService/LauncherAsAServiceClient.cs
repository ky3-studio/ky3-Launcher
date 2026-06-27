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
using Launcher.Web.Launcher.Redeem;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Launcher.Web.Launcher.LauncherAsAService;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class LauncherAsAServiceClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial LauncherAsAServiceClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<LauncherResponse> UploadAnnouncementAsync(string? accessToken, UploadAnnouncement uploadAnnouncement, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().AnnouncementUpload())
            .SetAccessToken(accessToken)
            .PostJson(uploadAnnouncement);

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse> CdnCompensationAsync(string? accessToken, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().CdnCompensation(days))
            .SetAccessToken(accessToken)
            .Get();

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse> CdnDesignationAsync(string? accessToken, string userName, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().CdnDesignation(userName, days))
            .SetAccessToken(accessToken)
            .Get();

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<RedeemGenerateResult>> GenerateRedeemCodesAsync(string? accessToken, RedeemGenerateRequest request, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().RedeemCodeGenerate())
            .SetAccessToken(accessToken)
            .PostJson(request);

        LauncherResponse<RedeemGenerateResult>? resp = await builder
            .SendAsync<LauncherResponse<RedeemGenerateResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}