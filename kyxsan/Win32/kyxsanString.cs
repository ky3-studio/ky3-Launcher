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

internal sealed unsafe class kyxsanString
{
    private readonly ObjectReference<Vftbl> objRef;

    public kyxsanString(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public string? Value
    {
        get
        {
            PCWSTR buffer;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetBuffer(objRef.ThisPtr, &buffer));
            return buffer.Value is null ? null : MemoryMarshal.CreateReadOnlySpanFromNullTerminated(buffer).ToString();
        }
    }

    public static kyxsanString AttachAbi(ref nint abi)
    {
        return new(ObjectReference<Vftbl>.Attach(ref abi, typeof(Vftbl).GUID));
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanString)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR*, HRESULT> GetBuffer;
#pragma warning restore CS0649
    }
}