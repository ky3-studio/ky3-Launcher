//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.UI.Windowing;
using Launcher.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Launcher.Win32;

internal readonly unsafe struct LauncherNativeWindowSubclassCallback
{
    private readonly delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, GCHandle<XamlWindowSubclass>, LRESULT*, BOOL> value;

    public LauncherNativeWindowSubclassCallback(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, GCHandle<XamlWindowSubclass>, LRESULT*, BOOL> value)
    {
        this.value = value;
    }

    public static LauncherNativeWindowSubclassCallback Create(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, GCHandle<XamlWindowSubclass>, LRESULT*, BOOL> method)
    {
        return new(method);
    }
}
