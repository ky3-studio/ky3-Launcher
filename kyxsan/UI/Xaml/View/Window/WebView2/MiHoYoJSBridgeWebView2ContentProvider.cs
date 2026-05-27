//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using kyxsan.Service.Notification;
using kyxsan.Service.User;
using kyxsan.Web.Bridge;
using Windows.Graphics;

namespace kyxsan.UI.Xaml.View.Window.WebView2;

[DependencyProperty<IJSBridgeUriSourceProvider>("SourceProvider")]
internal sealed partial class MiHoYoJSBridgeWebView2ContentProvider : DependencyObject, IWebView2ContentProvider
{
    private MiHoYoJSBridge? jsBridge;

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    public async ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);

        if (SourceProvider is null)
        {
            return;
        }

        IMessenger messenger = serviceProvider.GetRequiredService<IMessenger>();
        if (await serviceProvider.GetRequiredService<IUserService>().GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        await serviceProvider.GetRequiredService<ITaskContext>().SwitchToMainThreadAsync();
        string? source = SourceProvider.GetSource(userAndUid);
        if (!string.IsNullOrEmpty(source))
        {
            CoreWebView2Navigator navigator = new(CoreWebView2);
            await navigator.NavigateAsync("about:blank").ConfigureAwait(true);

            try
            {
                await CoreWebView2.Profile.ClearBrowsingDataAsync();
            }
            catch (InvalidCastException)
            {
                messenger.Send(InfoBarMessage.Warning(SH.ViewControlWebViewerCoreWebView2ProfileQueryInterfaceFailed));
                await CoreWebView2.DeleteCookiesAsync(userAndUid.IsOversea).ConfigureAwait(true);
            }

            CoreWebView2
                .SetCookie(userAndUid.User.CookieToken, userAndUid.User.LToken, userAndUid.IsOversea)
                .SetMobileUserAgent(userAndUid.IsOversea);
            jsBridge = SourceProvider.CreateJSBridge(serviceProvider, CoreWebView2, userAndUid);

            await navigator.NavigateAsync(source).ConfigureAwait(true);

            try
            {
                await CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.BrowsingHistory);
            }
            catch (InvalidCastException)
            {
                messenger.Send(InfoBarMessage.Warning(SH.ViewControlWebViewerCoreWebView2ProfileQueryInterfaceFailed));
                await CoreWebView2.DeleteCookiesAsync(userAndUid.IsOversea).ConfigureAwait(true);
            }
        }
    }

    public void Unload()
    {
        jsBridge?.Detach();
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        return WebView2WindowPosition.Vertical(parentRect, parentDpi);
    }
}