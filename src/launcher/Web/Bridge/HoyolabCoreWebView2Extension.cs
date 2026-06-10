//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using kyxsan.Web.Hoyolab;
using kyxsan.Web.WebView2;

namespace kyxsan.Web.Bridge;

internal static class HoyolabCoreWebView2Extension
{
    extension(CoreWebView2 webView)
    {
        public ValueTask DeleteCookiesAsync(bool isOversea)
        {
            return webView.DeleteCookiesAsync(isOversea ? ".hoyolab.com" : ".mihoyo.com");
        }

        public CoreWebView2 SetMobileUserAgentChinese()
        {
            webView.Settings.UserAgent = HoyolabOptions.MobileUserAgent;
            return webView;
        }

        public CoreWebView2 SetMobileUserAgentOversea()
        {
            webView.Settings.UserAgent = HoyolabOptions.MobileUserAgentOversea;
            return webView;
        }

        public CoreWebView2? SetMobileUserAgent(bool isOversea)
        {
            return isOversea
                ? webView.SetMobileUserAgentOversea()
                : webView.SetMobileUserAgentChinese();
        }

        public CoreWebView2 SetCookie(Cookie? cookieToken = null, Cookie? lToken = null, bool isOversea = false)
        {
            CoreWebView2CookieManager cookieManager = webView.CookieManager;

            if (cookieToken is not null)
            {
                cookieManager
                    .AddMihoyoCookie(Cookie.ACCOUNT_ID, cookieToken, isOversea)
                    .AddMihoyoCookie(Cookie.COOKIE_TOKEN, cookieToken, isOversea);

                if (lToken is not null)
                {
                    cookieManager
                        .AddMihoyoCookie(Cookie.LTUID, lToken, isOversea)
                        .AddMihoyoCookie(Cookie.LTOKEN, lToken, isOversea);
                }
            }

            return webView;
        }
    }

    extension(CoreWebView2CookieManager manager)
    {
        private CoreWebView2CookieManager AddMihoyoCookie(string name, Cookie cookie, bool isOversea = false)
        {
            string domain = isOversea ? ".hoyolab.com" : ".mihoyo.com";
            manager.AddOrUpdateCookie(manager.CreateCookie(name, cookie[name], domain, "/"));
            return manager;
        }
    }
}