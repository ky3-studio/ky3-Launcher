//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Launcher.UI.Windowing;
using Launcher.Win32.Foundation;

namespace Launcher.Core.LifeCycle;

internal static class CurrentXamlWindowReferenceExtension
{
    extension(ICurrentXamlWindowReference reference)
    {
        public XamlRoot? XamlRoot
        {
            get
            {
                try
                {
                    return reference.Window?.Content?.XamlRoot;
                }
                catch (ObjectDisposedException)
                {
                    return null;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    return null;
                }
            }
        }

        public HWND WindowHandle { get => reference.Window?.GetWindowHandle() ?? default; }

        public ElementTheme RequestedTheme
        {
            get
            {
                try
                {
                    return reference.Window?.Content is FrameworkElement element ? element.RequestedTheme : ElementTheme.Default;
                }
                catch (ObjectDisposedException)
                {
                    return ElementTheme.Default;
                }
            }
        }
    }
}
