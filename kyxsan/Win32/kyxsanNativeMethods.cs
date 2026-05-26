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

namespace kyxsan.Win32;

internal static unsafe class kyxsanNativeMethods
{
    // ReSharper disable InconsistentNaming
    public const string IID_IkyxsanString = "F1F44E9E-858D-4746-B44E-213A1DDA4510";

    public const string IID_IkyxsanNativeLoopbackSupport = "8607ACE4-313C-4C26-B1FB-CA11173B6953";
    public const string IID_IkyxsanNativeLoopbackSupport2 = "A5D67980-F495-4F52-BEF0-27D047E20174";
    public const string IID_IkyxsanNativeRegistryNotification = "EF118E91-8AD9-4C27-997D-DAF8910B34BE";
    public const string IID_IkyxsanNativeWindowSubclass = "9631921E-A6CA-4150-9939-99B5467B2FD6";
    public const string IID_IkyxsanNativeWindowSubclass2 = "D3D8B1C9-C83D-472B-AE9C-B65618B0F3AE";
    public const string IID_IkyxsanNativeWindowNonRude = "A0AD4485-702B-44B6-B48E-F82240EBBAEF";
    public const string IID_IkyxsanNative = "D00F73FF-A1C7-4091-8CB6-D90991DD40CB";
    public const string IID_IkyxsanNativeDeviceCapabilities = "1920EFA1-9953-4260-AFB1-35B1672758C1";
    public const string IID_IkyxsanNativePhysicalDrive = "F90535D7-AFF6-4008-BA8C-15C47FC9660D";
    public const string IID_IkyxsanNativeLogicalDrive = "D8E6C944-C964-4909-8BFF-901C40670DFF";
    public const string IID_IkyxsanNative2 = "338487EE-9592-4171-89DD-1E6B9EDB2C8E";
    public const string IID_IkyxsanNativeInputLowLevelKeyboardSource = "8628902F-835C-4293-8580-794EC9BCCB98";
    public const string IID_IkyxsanNative3 = "135FACE1-3184-4D12-B4D0-21FFB6A88D25";
    public const string IID_IkyxsanNativeFileSystem = "FDD58117-0C7F-44D8-A7A2-8B1766474A93";
    public const string IID_IkyxsanNativeFileSystem2 = "62616943-38e6-4bbb-84d1-dab847cb2145";
    public const string IID_IkyxsanNativeFileSystem3 = "6DBCFC5C-2555-44DB-AF9D-2A2628CF726E";
    public const string IID_IkyxsanNativeFileSystem4 = "FB4825F4-3A7F-47A1-8923-32933CB7DE92";
    public const string IID_IkyxsanNative4 = "27942FBE-322F-4157-9B8C-A38FDB827B05";
    public const string IID_IkyxsanNativeNotifyIcon = "C2203FA4-BB97-47E0-8F72-C96879E4CB11";
    public const string IID_IkyxsanNative5 = "7B4D20F1-4DAD-492E-8485-B4701A2ED19B";
    public const string IID_IkyxsanNativeHotKeyAction = "8C776674-9910-4721-A764-97BDB791F719";
    public const string IID_IkyxsanNative6 = "B68CABFA-C55A-49FA-8C51-21615C594E2B";
    public const string IID_IkyxsanNativeProcess = "13A2B1A2-03CB-46E1-AED5-6AFEA2DFFB39";
    public const string IID_IkyxsanNative7 = "B7A49A20-D9E2-43FD-9637-E80190443ABE";
    public const string IID_IkyxsanPrivate = "1A6980D9-EB36-4E3E-86E7-3665C57C6E8D";
    public const string IID_IkyxsanPrivate2 = "4E5D37CF-5F38-4FF2-9059-DF39CA696365";

    // ReSharper restore InconsistentNaming
    public const string DllName = "kyxsan.Native.dll";

    public static kyxsanNative kyxsanCreateInstance()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(kyxsanCreateInstance((kyxsanNative.Vftbl**)&pv));
        return new(ObjectReference<kyxsanNative.Vftbl>.Attach(ref pv, typeof(kyxsanNative.Vftbl).GUID));
    }

    public static BOOL IsWin32(HRESULT hr, WIN32_ERROR error)
    {
        return kyxsanHResultIsWin32(hr, error);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, EntryPoint = "HutaoCreateInstance")]
    private static extern HRESULT kyxsanCreateInstance(kyxsanNative.Vftbl** ppv);

    [DllImport(DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true, EntryPoint = "HutaoHResultIsWin32")]
    private static extern BOOL kyxsanHResultIsWin32(HRESULT hr, WIN32_ERROR error);
}