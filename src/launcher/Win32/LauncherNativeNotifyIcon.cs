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
using WinRT;
using WinRT.Interop;

namespace Launcher.Win32;

internal sealed unsafe class LauncherNativeNotifyIcon
{
    private readonly ObjectReference<Vftbl> objRef;

    public LauncherNativeNotifyIcon(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public BOOL IsPromoted
    {
        get
        {
            BOOL promoted = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsPromoted(objRef.ThisPtr, &promoted));
            return promoted;
        }
    }

    public void Create(LauncherNativeNotifyIconCallback callback, GCHandle<NotifyIconController> userData, ReadOnlySpan<char> tip)
    {
        fixed (char* pTip = tip)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.Create(objRef.ThisPtr, callback, userData, pTip));
        }
    }

    public void Recreate(ReadOnlySpan<char> tip)
    {
        fixed (char* pTip = tip)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.Recreate(objRef.ThisPtr, pTip));
        }
    }

    public void Destroy()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Destroy(objRef.ThisPtr));
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNativeNotifyIcon)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeNotifyIconCallback, GCHandle<NotifyIconController>, PCWSTR, HRESULT> Create;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> Recreate;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Destroy;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsPromoted;
#pragma warning restore CS0649
    }
}