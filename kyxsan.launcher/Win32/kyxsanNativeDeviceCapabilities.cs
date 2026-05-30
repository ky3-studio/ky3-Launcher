//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace kyxsan.Win32;

internal sealed unsafe class kyxsanNativeDeviceCapabilities
{
    private readonly ObjectReference<Vftbl> objRef;

    public kyxsanNativeDeviceCapabilities(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public int GetPrimaryScreenVerticalRefreshRate()
    {
        int refreshRate = 0;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.GetPrimaryScreenVerticalRefreshRate(objRef.ThisPtr, &refreshRate));
        return refreshRate;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeDeviceCapabilities)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, int*, HRESULT> GetPrimaryScreenVerticalRefreshRate;
#pragma warning restore CS0649
    }
}