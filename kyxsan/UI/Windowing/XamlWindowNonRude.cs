//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using System.Runtime.InteropServices;

namespace kyxsan.UI.Windowing;

internal sealed partial class XamlWindowNonRude : IDisposable
{
    private readonly kyxsanNativeWindowNonRude native;

    public XamlWindowNonRude(HWND hwnd)
    {
        native = kyxsanNative.Instance.MakeWindowNonRude(hwnd);
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
            if (kyxsanNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_MAPPED_ALIGNMENT))
            {
                return;
            }

            throw;
        }
    }
}