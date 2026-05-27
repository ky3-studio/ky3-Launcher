//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using kyxsan.Win32.Foundation;
using System.Diagnostics;

namespace kyxsan.Web.WebView2;

internal static class WebView2Extension
{
    extension(CoreWebView2 webView)
    {
        [Conditional("RELEASE")]
        public void DisableDevToolsForReleaseBuild()
        {
            CoreWebView2Settings settings = webView.Settings;
            settings.AreDefaultContextMenusEnabled = false;
            settings.AreDevToolsEnabled = false;

            try
            {
                settings.AreBrowserAcceleratorKeysEnabled = false; // ICoreWebView2Settings3
            }
            catch (Exception ex)
            {
                if (ex.HResult is HRESULT.E_NOINTERFACE)
                {
                    return;
                }

                throw;
            }
        }

        public void DisableAutoCompletion()
        {
            CoreWebView2Settings settings = webView.Settings;
            settings.IsGeneralAutofillEnabled = false;
            settings.IsPasswordAutosaveEnabled = false;
        }

        public async ValueTask DeleteCookiesAsync(string url)
        {
            CoreWebView2CookieManager manager = webView.CookieManager;
            IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync(url);
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }
        }
    }
}