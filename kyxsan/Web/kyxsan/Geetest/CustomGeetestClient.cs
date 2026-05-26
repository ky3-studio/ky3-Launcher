//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Service;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Collections.Frozen;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace kyxsan.Web.kyxsan.Geetest;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class CustomGeetestClient
{
    private static readonly FrozenSet<string> ImpossibleHosts =
    [
        "webstatic.mihoyo.com",
        "www.miyoushe.com",
    ];

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial CustomGeetestClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<GeetestResponse> VerifyAsync(string gt, string challenge, CancellationToken token)
    {
        string template = appOptions.GeetestCustomCompositeUrl.Value;

        try
        {
            UriBuilder uriBuilder = new(template);
            if (ImpossibleHosts.Contains(uriBuilder.Host))
            {
                await taskContext.SwitchToMainThreadAsync();
                appOptions.GeetestCustomCompositeUrl.Value = string.Empty;
                return GeetestResponse.InternalFailure;
            }
        }
        catch
        {
            return GeetestResponse.InternalFailure;
        }

        string url;
        try
        {
            CompositeFormat format = CompositeFormat.Parse(template);
            if (format.MinimumArgumentCount < 2)
            {
                return GeetestResponse.InternalFailure;
            }

            url = string.Format(CultureInfo.CurrentCulture, format.Format, gt, challenge);
        }
        catch (FormatException)
        {
            return GeetestResponse.InternalFailure;
        }

        if (string.IsNullOrEmpty(template) || !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return GeetestResponse.InternalFailure;
        }

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(uri)
            .Get();

        GeetestResponse? resp = await builder
            .SendAsync<GeetestResponse>(httpClient, false, token)
            .ConfigureAwait(false);

        return resp ?? GeetestResponse.InternalFailure;
    }
}