//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Win32;
using Launcher.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Launcher.UI.Windowing;

internal sealed partial class XamlWindowNonRude : IDisposable
{
    private readonly LauncherNativeWindowNonRude native;

    public XamlWindowNonRude(HWND hwnd)
    {
        native = LauncherNative.Instance.MakeWindowNonRude(hwnd);
        native.Attach();
    }

    public void Dispose()
    {
        try
        {
            native.Detach();
        }
        catch (COMException ex)
        {
            if (LauncherNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_MAPPED_ALIGNMENT))
            {
                return;
            }

            throw;
        }
    }
}