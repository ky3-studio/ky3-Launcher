//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Core.IO.Http;
using kyxsan.Web.Endpoint.kyxsan;
using kyxsan.Web.Enka.Model;
using kyxsan.Web.Hoyolab;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace kyxsan.Web.Enka;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class EnkaClient
{
    private const string EnkaAPI = "https://enka.network/api/uid/{0}";
    private const string EnkaInfoAPI = "https://enka.network/api/uid/{0}?info";

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial EnkaClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public ValueTask<EnkaResponse?> GetForwardPlayerInfoAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        string url = kyxsanEndpointsFactory.Create().EnkaPlayerInfo(playerUid);
        return TryGetEnkaResponseAsync(url, true, token);
    }

    public ValueTask<EnkaResponse?> GetPlayerInfoAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        return TryGetEnkaResponseAsync(string.Format(CultureInfo.CurrentCulture, EnkaInfoAPI, playerUid), false, token);
    }

    public ValueTask<EnkaResponse?> GetForwardDataAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        string url = kyxsanEndpointsFactory.Create().Enka(playerUid);
        return TryGetEnkaResponseAsync(url, true, token);
    }

    public ValueTask<EnkaResponse?> GetDataAsync(in PlayerUid playerUid, CancellationToken token = default)
    {
        return TryGetEnkaResponseAsync(string.Format(CultureInfo.CurrentCulture, EnkaAPI, playerUid), false, token);
    }

    private async ValueTask<EnkaResponse?> TryGetEnkaResponseAsync(string url, bool isForward, CancellationToken token = default)
    {
        try
        {
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetOptions(RetryHttpHandler.DisableRetry, true)
                .SetRequestUri(url)
                .Get();

            using (HttpResponseMessage response = await httpClient.SendAsync(builder.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EnkaResponse>(options, token).ConfigureAwait(false);
                }

                if (isForward)
                {
                    string content = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
                    if (content.Contains("nginx", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }

                // https://github.com/yoimiya-kokomi/miao-plugin/pull/441
                // Additionally, HTTP codes for UID requests:
                // 400 = wrong UID format
                // 404 = player does not exist(MHY server told that)
                // 429 = rate - limit
                // 424 = game maintenance / everything is broken after the update
                // 500 = general server error
                // 503 = I screwed up massively
                string message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => SH.WebEnkaResponseStatusCode400,
                    HttpStatusCode.NotFound => SH.WebEnkaResponseStatusCode404,
                    HttpStatusCode.FailedDependency => SH.WebEnkaResponseStatusCode424,
                    HttpStatusCode.TooManyRequests => SH.WebEnkaResponseStatusCode429,
                    HttpStatusCode.InternalServerError => SH.WebEnkaResponseStatusCode500,
                    HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout => SH.WebEnkaResponseStatusCode503,
                    _ => SH.WebEnkaResponseStatusCodeUnknown,
                };

                return new() { Message = message, };
            }
        }
        catch (Exception)
        {
            return null;
        }
    }
}