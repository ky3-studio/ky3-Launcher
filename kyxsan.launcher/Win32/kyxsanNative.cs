//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.UI.Input.HotKey;
using kyxsan.UI.Windowing;
using kyxsan.Win32.Foundation;
using System.Buffers;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace kyxsan.Win32;

internal sealed unsafe class kyxsanNative
{
    private readonly ObjectReference<Vftbl> objRef;
    private readonly ObjectReference<VftblPrivate>? objRefPrivate;
    private readonly ObjectReference<VftblPrivate2>? objRefPrivate2;
    private readonly ObjectReference<Vftbl2>? objRef2;
    private readonly ObjectReference<Vftbl3>? objRef3;
    private readonly ObjectReference<Vftbl4>? objRef4;
    private readonly ObjectReference<Vftbl5>? objRef5;
    private readonly ObjectReference<Vftbl6>? objRef6;
    private readonly ObjectReference<Vftbl7>? objRef7;

    public kyxsanNative(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
        objRef.TryAs(typeof(VftblPrivate).GUID, out objRefPrivate);
        objRef.TryAs(typeof(VftblPrivate2).GUID, out objRefPrivate2);
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
        objRef.TryAs(typeof(Vftbl3).GUID, out objRef3);
        objRef.TryAs(typeof(Vftbl4).GUID, out objRef4);
        objRef.TryAs(typeof(Vftbl5).GUID, out objRef5);
        objRef.TryAs(typeof(Vftbl6).GUID, out objRef6);
        objRef.TryAs(typeof(Vftbl7).GUID, out objRef7);
    }

    [field: MaybeNull]
    public static kyxsanNative Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
        {
            kyxsanNativeWilCallbacks.kyxsanInitializeWilCallbacks();
            return kyxsanNativeMethods.kyxsanCreateInstance();
        });
    }

    public static BOOL IsWin32(HRESULT hr, WIN32_ERROR error)
    {
        return kyxsanNativeMethods.IsWin32(hr, error);
    }

    public static BOOL IsWin32(HRESULT hr, ReadOnlySpan<WIN32_ERROR> errors)
    {
        foreach (ref readonly WIN32_ERROR error in errors)
        {
            if (kyxsanNativeMethods.IsWin32(hr, error))
            {
                return true;
            }
        }

        return false;
    }

    public BOOL IsCurrentWindowsVersionSupported()
    {
        kyxsanException.NotSupportedIf(objRefPrivate is null, "IkyxsanPrivate is not supported");

        BOOL isSupported = default;
        Marshal.ThrowExceptionForHR(objRefPrivate.Vftbl.IsCurrentWindowsVersionSupported(objRefPrivate.ThisPtr, &isSupported));
        return isSupported;
    }

    public kyxsanPrivateWindowsVersion GetCurrentWindowsVersion()
    {
        kyxsanException.NotSupportedIf(objRefPrivate is null, "IkyxsanPrivate is not supported");

        kyxsanPrivateWindowsVersion version = default;
        Marshal.ThrowExceptionForHR(objRefPrivate.Vftbl.GetWindowsVersion(objRefPrivate.ThisPtr, &version));
        return version;
    }

    public void ShowErrorMessage(ReadOnlySpan<char> title, ReadOnlySpan<char> message)
    {
        kyxsanException.NotSupportedIf(objRefPrivate is null, "IkyxsanPrivate is not supported");

        fixed (char* pTitle = title)
        {
            fixed (char* pMessage = message)
            {
                Marshal.ThrowExceptionForHR(objRefPrivate.Vftbl.ShowErrorMessage(objRefPrivate.ThisPtr, pTitle, pMessage));
            }
        }
    }

    public string ExchangeGameUidForIdentifier1820(ReadOnlySpan<char> gameUid)
    {
        if (gameUid.IsEmpty)
        {
            return string.Empty;
        }

        kyxsanException.NotSupportedIf(objRefPrivate2 is null, "IkyxsanPrivate2 is not supported");

        fixed (char* pGameUid = gameUid)
        {
            byte[] data = ArrayPool<byte>.Shared.Rent(gameUid.Length * 2);
            try
            {
                fixed (byte* identifier = data)
                {
                    Marshal.ThrowExceptionForHR(objRefPrivate2.Vftbl.ExchangeGameUidForIdentifier1820(objRefPrivate2.ThisPtr, pGameUid, identifier));
                    return Convert.ToBase64String(data.AsSpan(0, gameUid.Length * 2));
                }
            }
            catch (Exception ex)
            {
                ex.Data["GameUid"] = gameUid.ToString();
                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
    }

    public kyxsanNativeLoopbackSupport MakeLoopbackSupport()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeLoopbackSupport(objRef.ThisPtr, (kyxsanNativeLoopbackSupport.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeLoopbackSupport.Vftbl>.Attach(ref pv, typeof(kyxsanNativeLoopbackSupport.Vftbl).GUID));
    }

    public kyxsanNativeRegistryNotification MakeRegistryNotification(ReadOnlySpan<char> keyPath)
    {
        fixed (char* keyPathPtr = keyPath)
        {
            nint pv = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeRegistryNotification(objRef.ThisPtr, keyPathPtr, (kyxsanNativeRegistryNotification.Vftbl**)&pv));
            return new(ObjectReference<kyxsanNativeRegistryNotification.Vftbl>.Attach(ref pv, typeof(kyxsanNativeRegistryNotification.Vftbl).GUID));
        }
    }

    public kyxsanNativeWindowSubclass MakeWindowSubclass(HWND hWnd, kyxsanNativeWindowSubclassCallback callback, GCHandle<XamlWindowSubclass> userData)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeWindowSubclass(objRef.ThisPtr, hWnd, callback, userData, (kyxsanNativeWindowSubclass.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeWindowSubclass.Vftbl>.Attach(ref pv, typeof(kyxsanNativeWindowSubclass.Vftbl).GUID));
    }

    public kyxsanNativeWindowNonRude MakeWindowNonRude(HWND hWnd)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeWindowNonRude(objRef.ThisPtr, hWnd, (kyxsanNativeWindowNonRude.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeWindowNonRude.Vftbl>.Attach(ref pv, typeof(kyxsanNativeWindowNonRude.Vftbl).GUID));
    }

    public kyxsanNativeDeviceCapabilities MakeDeviceCapabilities()
    {
        kyxsanException.NotSupportedIf(objRef2 is null, "IkyxsanNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef2.Vftbl.MakeLoopbackSupport(objRef2.ThisPtr, (kyxsanNativeDeviceCapabilities.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeDeviceCapabilities.Vftbl>.Attach(ref pv, typeof(kyxsanNativeDeviceCapabilities.Vftbl).GUID));
    }

    public kyxsanNativePhysicalDrive MakePhysicalDrive()
    {
        kyxsanException.NotSupportedIf(objRef2 is null, "IkyxsanNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef2.Vftbl.MakePhysicalDrive(objRef2.ThisPtr, (kyxsanNativePhysicalDrive.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativePhysicalDrive.Vftbl>.Attach(ref pv, typeof(kyxsanNativePhysicalDrive.Vftbl).GUID));
    }

    public kyxsanNativeLogicalDrive MakeLogicalDrive()
    {
        kyxsanException.NotSupportedIf(objRef2 is null, "IkyxsanNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef2.Vftbl.MakeLogicalDrive(objRef2.ThisPtr, (kyxsanNativeLogicalDrive.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeLogicalDrive.Vftbl>.Attach(ref pv, typeof(kyxsanNativeLogicalDrive.Vftbl).GUID));
    }

    public kyxsanNativeInputLowLevelKeyboardSource MakeInputLowLevelKeyboardSource()
    {
        kyxsanException.NotSupportedIf(objRef3 is null, "IkyxsanNative3 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef3.Vftbl.MakeInputLowLevelKeyboardSource(objRef3.ThisPtr, (kyxsanNativeInputLowLevelKeyboardSource.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeInputLowLevelKeyboardSource.Vftbl>.Attach(ref pv, typeof(kyxsanNativeInputLowLevelKeyboardSource.Vftbl).GUID));
    }

    public kyxsanNativeFileSystem MakeFileSystem()
    {
        kyxsanException.NotSupportedIf(objRef4 is null, "IkyxsanNative4 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef4.Vftbl.MakeFileSystem(objRef4.ThisPtr, (kyxsanNativeFileSystem.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeFileSystem.Vftbl>.Attach(ref pv, typeof(kyxsanNativeFileSystem.Vftbl).GUID));
    }

    public kyxsanNativeNotifyIcon MakeNotifyIcon(ReadOnlySpan<char> iconPath, ref readonly Guid id)
    {
        kyxsanException.NotSupportedIf(objRef5 is null, "IkyxsanNative5 is not supported");
        fixed (char* pIconPath = iconPath)
        {
            fixed (Guid* pId = &id)
            {
                nint pv = default;
                Marshal.ThrowExceptionForHR(objRef5.Vftbl.MakeNotifyIcon(objRef5.ThisPtr, pIconPath, pId, (kyxsanNativeNotifyIcon.Vftbl**)&pv));
                return new(ObjectReference<kyxsanNativeNotifyIcon.Vftbl>.Attach(ref pv, typeof(kyxsanNativeNotifyIcon.Vftbl).GUID));
            }
        }
    }

    public kyxsanNativeHotKeyAction MakeHotKeyAction(kyxsanNativeHotKeyActionKind kind, kyxsanNativeHotKeyActionCallback callback, GCHandle<HotKeyCombination> userData)
    {
        kyxsanException.NotSupportedIf(objRef6 is null, "IkyxsanNative6 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef6.Vftbl.MakeHotKeyAction(objRef6.ThisPtr, kind, callback, userData, (kyxsanNativeHotKeyAction.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeHotKeyAction.Vftbl>.Attach(ref pv, typeof(kyxsanNativeHotKeyAction.Vftbl).GUID));
    }

    public kyxsanNativeProcess MakeProcess(kyxsanNativeProcessStartInfo info)
    {
        kyxsanException.NotSupportedIf(objRef7 is null, "IkyxsanNative7 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef7.Vftbl.MakeProcess(objRef7.ThisPtr, info, (kyxsanNativeProcess.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNativeProcess.Vftbl>.Attach(ref pv, typeof(kyxsanNativeProcess.Vftbl).GUID));
    }

#pragma warning disable CS0649
    [Guid(kyxsanNativeMethods.IID_IkyxsanNative)]
    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeLoopbackSupport.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, kyxsanNativeRegistryNotification.Vftbl**, HRESULT> MakeRegistryNotification;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, kyxsanNativeWindowSubclassCallback, GCHandle<XamlWindowSubclass>, kyxsanNativeWindowSubclass.Vftbl**, HRESULT> MakeWindowSubclass;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, kyxsanNativeWindowNonRude.Vftbl**, HRESULT> MakeWindowNonRude;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNative2)]
    private readonly struct Vftbl2
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeDeviceCapabilities.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativePhysicalDrive.Vftbl**, HRESULT> MakePhysicalDrive;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeLogicalDrive.Vftbl**, HRESULT> MakeLogicalDrive;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNative3)]
    private readonly struct Vftbl3
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeInputLowLevelKeyboardSource.Vftbl**, HRESULT> MakeInputLowLevelKeyboardSource;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNative4)]
    private readonly struct Vftbl4
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeFileSystem.Vftbl**, HRESULT> MakeFileSystem;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNative5)]
    private readonly struct Vftbl5
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, Guid*, kyxsanNativeNotifyIcon.Vftbl**, HRESULT> MakeNotifyIcon;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNative6)]
    private readonly struct Vftbl6
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeHotKeyActionKind, kyxsanNativeHotKeyActionCallback, GCHandle<HotKeyCombination>, kyxsanNativeHotKeyAction.Vftbl**, HRESULT> MakeHotKeyAction;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNative7)]
    private readonly struct Vftbl7
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanNativeProcessStartInfo, kyxsanNativeProcess.Vftbl**, HRESULT> MakeProcess;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanPrivate)]
    private readonly struct VftblPrivate
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsCurrentWindowsVersionSupported;
        internal readonly delegate* unmanaged[Stdcall]<nint, kyxsanPrivateWindowsVersion*, HRESULT> GetWindowsVersion;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> ShowErrorMessage;
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanPrivate2)]
    private readonly struct VftblPrivate2
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, byte*, HRESULT> ExchangeGameUidForIdentifier1820;
    }
#pragma warning restore CS0649
}