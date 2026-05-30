// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace kyxsan.Core.Shell;

[SupportedOSPlatform("windows")]
static partial class NativeMethods
{
    [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr ShellExecuteW(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShow);

    [DllImport("kernel32.dll")]
    public static extern uint GetLastError();

    private static bool IsAdministrator()
    {
        try
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    public static bool RestartAsAdministrator()
    {
        if (IsAdministrator())
        {
            return true;
        }

        try
        {
            Bootstrap.UseNamedPipeRedirection();

            string exeName = Environment.ProcessPath ?? string.Empty;
            IntPtr hInst = ShellExecuteW(IntPtr.Zero, "runas", exeName, string.Empty, string.Empty, 1);

            if (hInst == IntPtr.Zero || hInst.ToInt64() <= 32)
            {
                uint errorCode = GetLastError();
                throw new Win32Exception((int)errorCode);
            }

            Application.Current.Exit();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool RestartAsAdministratorAtStart()
    {
        if (IsAdministrator())
        {
            return true;
        }

        try
        {
            string exeName = Environment.ProcessPath ?? string.Empty;
            IntPtr hInst = ShellExecuteW(IntPtr.Zero, "runas", exeName, string.Empty, string.Empty, 1);

            if (hInst == IntPtr.Zero || hInst.ToInt64() <= 32)
            {
                uint errorCode = GetLastError();
                throw new Win32Exception((int)errorCode);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}