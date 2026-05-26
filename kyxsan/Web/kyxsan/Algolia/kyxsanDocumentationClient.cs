//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace kyxsan.Web.kyxsan.Algolia;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanDocumentationClient
{
    private const string AlgoliaApiKey = "36f3346b302103834b15cfbb221ee810";
    private const string AlgoliaApplicationId = "626H2LTAQH";
    private const string AlgolianetIndexesQueries = $"https://626H2LTAQH-2.algolianet.com/1/indexes/*/queries";

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanDocumentationClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<AlgoliaResponse?> QueryAsync(string query, string language, CancellationToken token = default)
    {
        AlgoliaRequestsWrapper data = new()
        {
            Requests =
            [
                new AlgoliaRequest
                {
                    Query = query,
                    IndexName = "kyxsan",
                    Params = $"""facetFilters=["lang:{language}"]""",
                },
            ],
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(AlgolianetIndexesQueries)
            .SetHeader("x-algolia-api-key", AlgoliaApiKey)
            .SetHeader("x-algolia-application-id", AlgoliaApplicationId)
            .PostJson(data);

        return await builder.SendAsync<AlgoliaResponse>(httpClient, token).ConfigureAwait(false);
    }
}