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
using kyxsan.UI.Xaml.View.Window.WebView2;
using kyxsan.Web.Bridge;

namespace kyxsan.ViewModel.User;

internal sealed class SignInJSBridgeUriSourceProvider : DependencyObject, IJSBridgeUriSourceProvider
{
    public MiHoYoJSBridge CreateJSBridge(IServiceProvider serviceProvider, CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        return userAndUid.IsOversea
            ? ActivatorUtilities.CreateInstance<SignInJSBridgeOversea>(serviceProvider, coreWebView2, userAndUid)
            : ActivatorUtilities.CreateInstance<SignInJSBridge>(serviceProvider, coreWebView2, userAndUid);
    }

    public string GetSource(UserAndUid userAndUid)
    {
        return userAndUid.IsOversea
            ? "https://act.hoyolab.com/ys/event/signin-sea-v3/index.html?act_id=e202102251931481"
            : "https://act.mihoyo.com/bbs/event/signin/hk4e/index.html?act_id=e202311201442471";
    }
}