//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.WindowsAndMessaging;

namespace kyxsan.Win32;

internal readonly unsafe struct kyxsanNativeInputLowLevelKeyboardSourceCallback
{
    private readonly delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> value;

    public kyxsanNativeInputLowLevelKeyboardSourceCallback(delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> value)
    {
        this.value = value;
    }

    public static kyxsanNativeInputLowLevelKeyboardSourceCallback Create(delegate* unmanaged[Stdcall]<uint, KBDLLHOOKSTRUCT*, BOOL> method)
    {
        return new(method);
    }
}