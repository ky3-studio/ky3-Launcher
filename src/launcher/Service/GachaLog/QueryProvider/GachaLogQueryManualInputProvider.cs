//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Factory.ContentDialog;
using kyxsan.UI.Xaml.View.Dialog;
using System.Collections.Specialized;
using System.Web;

namespace kyxsan.Service.GachaLog.QueryProvider;

[Service(ServiceLifetime.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOptionKind.ManualInput)]
internal sealed partial class GachaLogQueryManualInputProvider : IGachaLogQueryProvider
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly CultureOptions cultureOptions;

    [GeneratedConstructor]
    public partial GachaLogQueryManualInputProvider(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        GachaLogUrlDialog dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogUrlDialog>(serviceProvider).ConfigureAwait(false);
        if (await dialog.GetInputUrlAsync().ConfigureAwait(false) is not (true, { } url))
        {
            return new(false, default);
        }

        if ((AfterLast(url, "index.html") ?? AfterLast(url, "getGachaLog")) is not { } queryString)
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderManualInputInvalid));
        }

        NameValueCollection query = HttpUtility.ParseQueryString(queryString);
        if (!query.TryGetSingleValue("auth_appid", out string? appId) || appId is not "webview_gacha")
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderManualInputInvalid));
        }

        if (!query.TryGetSingleValue("lang", out string? queryLanguageCode) || !LocaleNames.LanguageCodeFitsCurrentLocale(queryLanguageCode, cultureOptions.LocaleName))
        {
            string message = SH.FormatServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale(queryLanguageCode, cultureOptions.LanguageCode);
            return new(false, GachaLogQuery.Invalid(message));
        }

        return new(true, new(url));
    }

    private static string? AfterLast(string url, string match)
    {
        ReadOnlySpan<char> urlSpan = url;

        int index = urlSpan.LastIndexOf(match);
        if (index >= 0)
        {
            index += match.Length;
            return urlSpan[index..].ToString();
        }

        return default;
    }
}
