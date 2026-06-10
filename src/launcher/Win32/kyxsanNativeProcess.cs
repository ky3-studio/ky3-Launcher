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

internal sealed unsafe class kyxsanNativeProcess
{
    private readonly ObjectReference<Vftbl> objRef;

    public kyxsanNativeProcess(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public uint Id
    {
        get
        {
            uint id;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetId(objRef.ThisPtr, &id));
            return id;
        }
    }

    public HANDLE ProcessHandle
    {
        get
        {
            HANDLE hProcess;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetProcessHandle(objRef.ThisPtr, &hProcess));
            return hProcess;
        }
    }

    public HWND MainWindowHandle
    {
        get
        {
            HWND hWnd;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetMainWindowHandle(objRef.ThisPtr, &hWnd));
            return hWnd;
        }
    }

    public void Start()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Start(objRef.ThisPtr));
    }

    public void ResumeMainThread()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.ResumeMainThread(objRef.ThisPtr));
    }

    public void WaitForExit()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.WaitForExit(objRef.ThisPtr));
    }

    public void Kill()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Kill(objRef.ThisPtr));
    }

    public BOOL GetExitCodeProcess(out uint exitCode)
    {
        fixed (uint* pExitCode = &exitCode)
        {
            BOOL isRunning;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetExitCodeProcess(objRef.ThisPtr, &isRunning, pExitCode));
            return isRunning;
        }
    }

    [Guid(kyxsanNativeMethods.IID_IkyxsanNativeProcess)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Start;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ResumeMainThread;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> WaitForExit;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Kill;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> GetId;
        internal readonly delegate* unmanaged[Stdcall]<nint, HANDLE*, HRESULT> GetProcessHandle;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND*, HRESULT> GetMainWindowHandle;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, uint*, HRESULT> GetExitCodeProcess;
#pragma warning restore CS0649
    }
}