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

namespace kyxsan.UI.Xaml;

internal static class FrameworkTheming
{
    public static void SetTheme(Theme theme)
    {
        Marshal.ThrowExceptionForHR(FrameworkThemingSetTheme(theme));
    }

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport(kyxsanNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT FrameworkThemingSetTheme(Theme theme);
}