//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Input.HotKey;
using kyxsan.Win32.Foundation;
using System.Runtime.InteropServices;

namespace kyxsan.Win32;

internal readonly unsafe struct kyxsanNativeHotKeyActionCallback
{
    private readonly delegate* unmanaged[Stdcall]<BOOL, GCHandle<HotKeyCombination>, void> value;

    public kyxsanNativeHotKeyActionCallback(delegate* unmanaged[Stdcall]<BOOL, GCHandle<HotKeyCombination>, void> value)
    {
        this.value = value;
    }

    public static kyxsanNativeHotKeyActionCallback Create(delegate* unmanaged[Stdcall]<BOOL, GCHandle<HotKeyCombination>, void> method)
    {
        return new(method);
    }
}