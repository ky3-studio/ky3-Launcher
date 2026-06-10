//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.Shell;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace kyxsan.Win32;

internal sealed unsafe class kyxsanNativeWindowSubclass
{
    private readonly ObjectReference<Vftbl2>? objRef2;

    public kyxsanNativeWindowSubclass(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<Vftbl2>? ObjRef2 { get => objRef2; }

    public void Attach()
    {
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.Attach(ObjRef.ThisPtr));
    }

    public void Detach()
    {
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.Detach(ObjRef.ThisPtr));
    }

    public void InitializeTaskbarProgress()
    {
        kyxsanException.NotSupportedIf(ObjRef2 is null, "IkyxsanNativeWindowSubclass2 is not supported");
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.InitializeTaskbarProgress(ObjRef2.ThisPtr));
    }

    public void SetTaskbarProgress(TBPFLAG flags, ulong value, ulong maximum)
    {
        kyxsanException.NotSupportedIf(ObjRef2 is null, "IkyxsanNativeWindowSubclass2 is not supported");
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.SetTaskbarProgress(ObjRef2.ThisPtr, flags, value, maximum));
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeWindowSubclass)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Attach;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Detach;
#pragma warning restore CS0649
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeWindowSubclass2)]
    internal readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> InitializeTaskbarProgress;
        internal readonly delegate* unmanaged[Stdcall]<nint, TBPFLAG, ulong, ulong, HRESULT> SetTaskbarProgress;
#pragma warning restore CS0649
    }
}