//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using kyxsan.ViewModel.User;

namespace kyxsan.Web.Bridge;

internal sealed class SignInJSBridge : MiHoYoJSBridge
{
    public SignInJSBridge(IServiceProvider serviceProvider, CoreWebView2 webView, UserAndUid userAndUid)
        : base(serviceProvider, webView, userAndUid)
    {
    }

    protected override void GetHttpRequestHeaderOverride(Dictionary<string, string> headers)
    {
        headers["x-rpc-client_type"] = "2";
    }
}