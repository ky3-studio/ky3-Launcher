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
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace kyxsan.Win32;

internal sealed unsafe class kyxsanNativeLoopbackSupport
{
    private readonly ObjectReference<Vftbl> objRef;
    private readonly ObjectReference<Vftbl2>? objRef2;

    public kyxsanNativeLoopbackSupport(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
    }

    public BOOL IsPublicFirewallEnabled
    {
        get
        {
            kyxsanException.NotSupportedIf(objRef2 is null, "IkyxsanNativeLoopbackSupport2 is not supported");

            BOOL isEnabled = default;
            Marshal.ThrowExceptionForHR(objRef2.Vftbl.IsPublicFirewallEnabled(objRef2.ThisPtr, &isEnabled));
            return isEnabled;
        }
    }

    public BOOL IsEnabled(ReadOnlySpan<char> familyName, out string? sid)
    {
        fixed (char* pFamilyName = familyName)
        {
            nint pSid = default;
            BOOL enabled = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsEnabled(objRef.ThisPtr, pFamilyName, (kyxsanString.Vftbl**)&pSid, &enabled));
            sid = kyxsanString.AttachAbi(ref pSid).Value;
            return enabled;
        }
    }

    public void Enable(ReadOnlySpan<char> sid)
    {
        fixed (char* pSid = sid)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.Enable(objRef.ThisPtr, pSid));
        }
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeLoopbackSupport)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, kyxsanString.Vftbl**, BOOL*, HRESULT> IsEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> Enable;
#pragma warning restore CS0649
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeLoopbackSupport2)]
    internal readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsPublicFirewallEnabled;
#pragma warning restore CS0649
    }
}