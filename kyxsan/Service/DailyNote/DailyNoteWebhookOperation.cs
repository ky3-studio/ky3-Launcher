//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Web.Hoyolab;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Net.Http;
using WebDailyNote = kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace kyxsan.Service.DailyNote;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class DailyNoteWebhookOperation
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly DailyNoteOptions dailyNoteOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial DailyNoteWebhookOperation(IServiceProvider serviceProvider, HttpClient httpClient);

    public void TryPostDailyNoteToWebhook(PlayerUid playerUid, WebDailyNote dailyNote)
    {
        string? targetUrl = dailyNoteOptions.WebhookUrl.Value;
        if (string.IsNullOrEmpty(targetUrl) || !Uri.TryCreate(targetUrl, UriKind.Absolute, out Uri? targetUri))
        {
            return;
        }

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(targetUri)
            .SetHeader("x-uid", $"{playerUid}")
            .PostJson(dailyNote);

        builder.Send(httpClient);
    }
}