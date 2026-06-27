//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.UI.Shell;
using Launcher.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Launcher.Win32;

internal readonly unsafe struct LauncherNativeNotifyIconCallback
{
    private readonly delegate* unmanaged[Stdcall]<LauncherNativeNotifyIconCallbackKind, RECT, POINT, GCHandle<NotifyIconController>, void> value;

    public LauncherNativeNotifyIconCallback(delegate* unmanaged[Stdcall]<LauncherNativeNotifyIconCallbackKind, RECT, POINT, GCHandle<NotifyIconController>, void> value)
    {
        this.value = value;
    }

    public static LauncherNativeNotifyIconCallback Create(delegate* unmanaged[Stdcall]<LauncherNativeNotifyIconCallbackKind, RECT, POINT, GCHandle<NotifyIconController>, void> method)
    {
        return new(method);
    }
}