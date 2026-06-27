//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.ExceptionService;
using Launcher.UI.Input.HotKey;
using Launcher.UI.Windowing;
using Launcher.Win32.Foundation;
using System.Buffers;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Launcher.Win32;

internal sealed unsafe class LauncherNative
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

    public LauncherNative(ObjectReference<Vftbl> objRef)
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
    public static LauncherNative Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
        {
            LauncherNativeWilCallbacks.LauncherInitializeWilCallbacks();
            return LauncherNativeMethods.LauncherCreateInstance();
        });
    }

    public static BOOL IsWin32(HRESULT hr, WIN32_ERROR error)
    {
        return LauncherNativeMethods.IsWin32(hr, error);
    }

    public static BOOL IsWin32(HRESULT hr, ReadOnlySpan<WIN32_ERROR> errors)
    {
        foreach (ref readonly WIN32_ERROR error in errors)
        {
            if (LauncherNativeMethods.IsWin32(hr, error))
            {
                return true;
            }
        }

        return false;
    }

    public BOOL IsCurrentWindowsVersionSupported()
    {
        LauncherException.NotSupportedIf(objRefPrivate is null, "ILauncherPrivate is not supported");

        BOOL isSupported = default;
        Marshal.ThrowExceptionForHR(objRefPrivate.Vftbl.IsCurrentWindowsVersionSupported(objRefPrivate.ThisPtr, &isSupported));
        return isSupported;
    }

    public LauncherPrivateWindowsVersion GetCurrentWindowsVersion()
    {
        LauncherException.NotSupportedIf(objRefPrivate is null, "ILauncherPrivate is not supported");

        LauncherPrivateWindowsVersion version = default;
        Marshal.ThrowExceptionForHR(objRefPrivate.Vftbl.GetWindowsVersion(objRefPrivate.ThisPtr, &version));
        return version;
    }

    public void ShowErrorMessage(ReadOnlySpan<char> title, ReadOnlySpan<char> message)
    {
        LauncherException.NotSupportedIf(objRefPrivate is null, "ILauncherPrivate is not supported");

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

        LauncherException.NotSupportedIf(objRefPrivate2 is null, "ILauncherPrivate2 is not supported");

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

    public LauncherNativeLoopbackSupport MakeLoopbackSupport()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeLoopbackSupport(objRef.ThisPtr, (LauncherNativeLoopbackSupport.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeLoopbackSupport.Vftbl>.Attach(ref pv, typeof(LauncherNativeLoopbackSupport.Vftbl).GUID));
    }

    public LauncherNativeRegistryNotification MakeRegistryNotification(ReadOnlySpan<char> keyPath)
    {
        fixed (char* keyPathPtr = keyPath)
        {
            nint pv = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeRegistryNotification(objRef.ThisPtr, keyPathPtr, (LauncherNativeRegistryNotification.Vftbl**)&pv));
            return new(ObjectReference<LauncherNativeRegistryNotification.Vftbl>.Attach(ref pv, typeof(LauncherNativeRegistryNotification.Vftbl).GUID));
        }
    }

    public LauncherNativeWindowSubclass MakeWindowSubclass(HWND hWnd, LauncherNativeWindowSubclassCallback callback, GCHandle<XamlWindowSubclass> userData)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeWindowSubclass(objRef.ThisPtr, hWnd, callback, userData, (LauncherNativeWindowSubclass.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeWindowSubclass.Vftbl>.Attach(ref pv, typeof(LauncherNativeWindowSubclass.Vftbl).GUID));
    }

    public LauncherNativeWindowNonRude MakeWindowNonRude(HWND hWnd)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.MakeWindowNonRude(objRef.ThisPtr, hWnd, (LauncherNativeWindowNonRude.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeWindowNonRude.Vftbl>.Attach(ref pv, typeof(LauncherNativeWindowNonRude.Vftbl).GUID));
    }

    public LauncherNativeDeviceCapabilities MakeDeviceCapabilities()
    {
        LauncherException.NotSupportedIf(objRef2 is null, "ILauncherNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef2.Vftbl.MakeLoopbackSupport(objRef2.ThisPtr, (LauncherNativeDeviceCapabilities.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeDeviceCapabilities.Vftbl>.Attach(ref pv, typeof(LauncherNativeDeviceCapabilities.Vftbl).GUID));
    }

    public LauncherNativePhysicalDrive MakePhysicalDrive()
    {
        LauncherException.NotSupportedIf(objRef2 is null, "ILauncherNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef2.Vftbl.MakePhysicalDrive(objRef2.ThisPtr, (LauncherNativePhysicalDrive.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativePhysicalDrive.Vftbl>.Attach(ref pv, typeof(LauncherNativePhysicalDrive.Vftbl).GUID));
    }

    public LauncherNativeLogicalDrive MakeLogicalDrive()
    {
        LauncherException.NotSupportedIf(objRef2 is null, "ILauncherNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef2.Vftbl.MakeLogicalDrive(objRef2.ThisPtr, (LauncherNativeLogicalDrive.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeLogicalDrive.Vftbl>.Attach(ref pv, typeof(LauncherNativeLogicalDrive.Vftbl).GUID));
    }

    public LauncherNativeInputLowLevelKeyboardSource MakeInputLowLevelKeyboardSource()
    {
        LauncherException.NotSupportedIf(objRef3 is null, "ILauncherNative3 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef3.Vftbl.MakeInputLowLevelKeyboardSource(objRef3.ThisPtr, (LauncherNativeInputLowLevelKeyboardSource.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeInputLowLevelKeyboardSource.Vftbl>.Attach(ref pv, typeof(LauncherNativeInputLowLevelKeyboardSource.Vftbl).GUID));
    }

    public LauncherNativeFileSystem MakeFileSystem()
    {
        LauncherException.NotSupportedIf(objRef4 is null, "ILauncherNative4 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef4.Vftbl.MakeFileSystem(objRef4.ThisPtr, (LauncherNativeFileSystem.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeFileSystem.Vftbl>.Attach(ref pv, typeof(LauncherNativeFileSystem.Vftbl).GUID));
    }

    public LauncherNativeNotifyIcon MakeNotifyIcon(ReadOnlySpan<char> iconPath, ref readonly Guid id)
    {
        LauncherException.NotSupportedIf(objRef5 is null, "ILauncherNative5 is not supported");
        fixed (char* pIconPath = iconPath)
        {
            fixed (Guid* pId = &id)
            {
                nint pv = default;
                Marshal.ThrowExceptionForHR(objRef5.Vftbl.MakeNotifyIcon(objRef5.ThisPtr, pIconPath, pId, (LauncherNativeNotifyIcon.Vftbl**)&pv));
                return new(ObjectReference<LauncherNativeNotifyIcon.Vftbl>.Attach(ref pv, typeof(LauncherNativeNotifyIcon.Vftbl).GUID));
            }
        }
    }

    public LauncherNativeHotKeyAction MakeHotKeyAction(LauncherNativeHotKeyActionKind kind, LauncherNativeHotKeyActionCallback callback, GCHandle<HotKeyCombination> userData)
    {
        LauncherException.NotSupportedIf(objRef6 is null, "ILauncherNative6 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef6.Vftbl.MakeHotKeyAction(objRef6.ThisPtr, kind, callback, userData, (LauncherNativeHotKeyAction.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeHotKeyAction.Vftbl>.Attach(ref pv, typeof(LauncherNativeHotKeyAction.Vftbl).GUID));
    }

    public LauncherNativeProcess MakeProcess(LauncherNativeProcessStartInfo info)
    {
        LauncherException.NotSupportedIf(objRef7 is null, "ILauncherNative7 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(objRef7.Vftbl.MakeProcess(objRef7.ThisPtr, info, (LauncherNativeProcess.Vftbl**)&pv));
        return new(ObjectReference<LauncherNativeProcess.Vftbl>.Attach(ref pv, typeof(LauncherNativeProcess.Vftbl).GUID));
    }

#pragma warning disable CS0649
    [Guid(LauncherNativeMethods.IID_ILauncherNative)]
    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeLoopbackSupport.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, LauncherNativeRegistryNotification.Vftbl**, HRESULT> MakeRegistryNotification;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, LauncherNativeWindowSubclassCallback, GCHandle<XamlWindowSubclass>, LauncherNativeWindowSubclass.Vftbl**, HRESULT> MakeWindowSubclass;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, LauncherNativeWindowNonRude.Vftbl**, HRESULT> MakeWindowNonRude;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNative2)]
    private readonly struct Vftbl2
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeDeviceCapabilities.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativePhysicalDrive.Vftbl**, HRESULT> MakePhysicalDrive;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeLogicalDrive.Vftbl**, HRESULT> MakeLogicalDrive;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNative3)]
    private readonly struct Vftbl3
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeInputLowLevelKeyboardSource.Vftbl**, HRESULT> MakeInputLowLevelKeyboardSource;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNative4)]
    private readonly struct Vftbl4
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeFileSystem.Vftbl**, HRESULT> MakeFileSystem;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNative5)]
    private readonly struct Vftbl5
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, Guid*, LauncherNativeNotifyIcon.Vftbl**, HRESULT> MakeNotifyIcon;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNative6)]
    private readonly struct Vftbl6
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeHotKeyActionKind, LauncherNativeHotKeyActionCallback, GCHandle<HotKeyCombination>, LauncherNativeHotKeyAction.Vftbl**, HRESULT> MakeHotKeyAction;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherNative7)]
    private readonly struct Vftbl7
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherNativeProcessStartInfo, LauncherNativeProcess.Vftbl**, HRESULT> MakeProcess;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherPrivate)]
    private readonly struct VftblPrivate
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsCurrentWindowsVersionSupported;
        internal readonly delegate* unmanaged[Stdcall]<nint, LauncherPrivateWindowsVersion*, HRESULT> GetWindowsVersion;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> ShowErrorMessage;
    }

    [Guid(LauncherNativeMethods.IID_ILauncherPrivate2)]
    private readonly struct VftblPrivate2
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, byte*, HRESULT> ExchangeGameUidForIdentifier1820;
    }
#pragma warning restore CS0649
}