//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Win32.Foundation;
using Launcher.Win32.UI.WindowsAndMessaging;

namespace Launcher.Win32;

internal readonly unsafe struct LauncherNativeInputLowLevelKeyboardSourceCallback
{
    private readonly delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> value;

    public LauncherNativeInputLowLevelKeyboardSourceCallback(delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> value)
    {
        this.value = value;
    }

    public static LauncherNativeInputLowLevelKeyboardSourceCallback Create(delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> method)
    {
        return new(method);
    }
}
