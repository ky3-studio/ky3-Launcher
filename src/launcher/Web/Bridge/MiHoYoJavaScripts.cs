//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Bridge;

internal static class MiHoYoJavaScripts
{
    /* lang=javascript */
    public const string InitializeJsInterfaceScript = """
        window.MiHoYoJSInterface = {
            postMessage: function(arg) { window.chrome.webview.postMessage(arg) },
            closePage: function() { this.postMessage('{"method":"closePage"}') },
        };
        """;

    /* lang=javascript */
    public const string HideScrollBarScript = """
        let hideStyle = document.createElement('style');
        hideStyle.innerHTML = '::-webkit-scrollbar{ display:none }';
        document.querySelector('body').appendChild(hideStyle);
        """;

    /* lang=javascript */
    public const string ConvertMouseToTouchScript = """
        function mouseListener (e, event) {
            let touch = new Touch({
                identifier: Date.now(),
                target: e.target,
                clientX: e.clientX,
                clientY: e.clientY,
                screenX: e.screenX,
                screenY: e.screenY,
                pageX: e.pageX,
                pageY: e.pageY,
            });
            let touchEvent = new TouchEvent(event, {
                cancelable: true,
                bubbles: true,
                touches: [touch],
                targetTouches: [touch],
                changedTouches: [touch],
            });
            e.target.dispatchEvent(touchEvent);
        }

        let mouseMoveListener = (e) => {
            mouseListener(e, 'touchmove'); 
        };

        let mouseUpListener = (e) => {
            mouseListener(e, 'touchend'); 
            document.removeEventListener('mousemove', mouseMoveListener);
            document.removeEventListener('mouseup', mouseUpListener);
        };

        let mouseDownListener = (e) => {
            mouseListener(e, 'touchstart'); 
            document.addEventListener('mousemove', mouseMoveListener);
            document.addEventListener('mouseup', mouseUpListener);
        };
        document.addEventListener('mousedown', mouseDownListener);
        """;
}