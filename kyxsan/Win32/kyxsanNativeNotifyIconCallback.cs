//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Shell;
using kyxsan.Win32.Foundation;
using System.Runtime.InteropServices;

namespace kyxsan.Win32;

internal readonly unsafe struct kyxsanNativeNotifyIconCallback
{
    private readonly delegate* unmanaged[Stdcall]<kyxsanNativeNotifyIconCallbackKind, RECT, POINT, GCHandle<NotifyIconController>, void> value;

    public kyxsanNativeNotifyIconCallback(delegate* unmanaged[Stdcall]<kyxsanNativeNotifyIconCallbackKind, RECT, POINT, GCHandle<NotifyIconController>, void> value)
    {
        this.value = value;
    }

    public static kyxsanNativeNotifyIconCallback Create(delegate* unmanaged[Stdcall]<kyxsanNativeNotifyIconCallbackKind, RECT, POINT, GCHandle<NotifyIconController>, void> method)
    {
        return new(method);
    }
}