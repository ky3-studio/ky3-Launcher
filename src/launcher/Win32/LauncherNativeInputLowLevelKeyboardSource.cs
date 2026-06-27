//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Launcher.Win32;

internal sealed unsafe class LauncherNativeInputLowLevelKeyboardSource
{
    private readonly ObjectReference<Vftbl> objRef;

    public LauncherNativeInputLowLevelKeyboardSource(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void Attach(LauncherNativeInputLowLevelKeyboardSourceCallback callback)
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Attach(objRef.ThisPtr, callback));
    }

    public void Detach(LauncherNativeInputLowLevelKeyboardSourceCallback callback)
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Detach(objRef.ThisPtr, callback));
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNativeInputLowLevelKeyboardSource)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeInputLowLevelKeyboardSourceCallback, HRESULT> Attach;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeInputLowLevelKeyboardSourceCallback, HRESULT> Detach;
#pragma warning restore CS0649
    }
}