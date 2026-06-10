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

internal sealed unsafe class kyxsanNativePhysicalDrive
{
    private readonly ObjectReference<Vftbl> objRef;

    public kyxsanNativePhysicalDrive(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public bool IsPathOnSolidStateDrive(string root)
    {
        BOOL isSSD;
        fixed (char* pRoot = root)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsPathOnSolidStateDrive(objRef.ThisPtr, pRoot, &isSSD));
        }

        return isSSD;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativePhysicalDrive)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, BOOL*, HRESULT> IsPathOnSolidStateDrive;
#pragma warning restore CS0649
    }
}