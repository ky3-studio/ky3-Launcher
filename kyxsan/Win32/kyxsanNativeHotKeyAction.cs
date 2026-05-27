//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace kyxsan.Win32;

internal sealed unsafe class kyxsanNativeHotKeyAction
{
    private readonly ObjectReference<Vftbl> objRef;

    public kyxsanNativeHotKeyAction(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public BOOL IsEnabled
    {
        get
        {
            BOOL isEnabled = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetIsEnabled(objRef.ThisPtr, &isEnabled));
            return isEnabled;
        }

        set
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.SetIsEnabled(objRef.ThisPtr, value));
        }
    }

    public static void InitializeBeforeSwitchCallback(kyxsanNativeHotKeyBeforeSwitchCallback callback)
    {
        Marshal.ThrowExceptionForHR(kyxsanNativeHotKeyInitializeBeforeSwitchCallback(callback));
    }

    public void Update(HOT_KEY_MODIFIERS modifiers, uint vk)
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Update(objRef.ThisPtr, modifiers, vk));
    }

    [DllImport(kyxsanNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, EntryPoint = "HutaoNativeHotKeyInitializeBeforeSwitchCallback")]
    private static extern HRESULT kyxsanNativeHotKeyInitializeBeforeSwitchCallback(kyxsanNativeHotKeyBeforeSwitchCallback callback);

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeHotKeyAction)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HOT_KEY_MODIFIERS, uint, HRESULT> Update;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> GetIsEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL, HRESULT> SetIsEnabled;
#pragma warning restore CS0649
    }
}