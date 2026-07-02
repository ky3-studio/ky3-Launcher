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

internal sealed unsafe class LauncherNativeLogicalDrive
{
    private readonly ObjectReference<Vftbl> objRef;

    public LauncherNativeLogicalDrive(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public long GetDiskFreeSpace(string path)
    {
        long bytes;
        fixed (char* pPath = path)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetDiskFreeSpace(objRef.ThisPtr, pPath, &bytes));
        }

        return bytes;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNativeLogicalDrive)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, long*, HRESULT> GetDiskFreeSpace;
#pragma warning restore CS0649
    }
}
