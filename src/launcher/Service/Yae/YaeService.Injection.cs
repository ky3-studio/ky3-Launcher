//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Core.Diagnostics;
using Launcher.Core.LifeCycle.InterProcess.Yae;
using Launcher.Service.Game;
using Launcher.Service.User;
using Launcher.Service.Yae.Achievement;
using Launcher.ViewModel.User;
using BindingUser = Launcher.ViewModel.User.User;
using Launcher.Web.Hoyolab.Passport;
using Launcher.Web.Response;
using Launcher.Win32.Foundation;
using System.Collections.Immutable;
using System.IO;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Launcher.Service.Yae;

internal sealed partial class YaeService
{
    private async ValueTask<ImmutableArray<YaeData>> InjectAndCollectAsync(
        string gameFilePath, string gameDirectory, string dllPath,
        TargetNativeConfiguration config, LaunchOptions launchOptions)
    {
        string cmdArgs = string.Empty;
        if (launchOptions.AreCommandLineArgumentsEnabled.Value)
        {
            cmdArgs = new CommandLineBuilder()
                .AppendIf(launchOptions.IsWindowed.Value, "-windowed")
                .AppendIf(launchOptions.IsBorderless.Value, "-popupwindow")
                .AppendIf(launchOptions.IsExclusive.Value, "-window-mode", "exclusive")
                .Append("-screen-fullscreen", launchOptions.IsFullScreen.Value ? "1" : "0")
                .AppendIf(launchOptions.IsScreenWidthEnabled.Value, "-screen-width", launchOptions.ScreenWidth.Value)
                .AppendIf(launchOptions.IsScreenHeightEnabled.Value, "-screen-height", launchOptions.ScreenHeight.Value)
                .AppendIf(launchOptions.IsMonitorEnabled.Value, "-monitor", launchOptions.Monitor.Value?.Value ?? 1)
                .ToString();
        }

        if (launchOptions.UsingHoyolabAccount.Value)
        {
            try
            {
                using IServiceScope authScope = serviceProvider.CreateScope();
                IUserService userService = authScope.ServiceProvider.GetRequiredService<IUserService>();

                string savedMid = launchOptions.SelectedHoyolabUserMid.Value;
                UserAndUid? userAndUid = null;
                if (!string.IsNullOrEmpty(savedMid))
                {
                    BindingUser? selectedUser = await userService.GetUserByMidAsync(savedMid).ConfigureAwait(false);
                    UserAndUid.TryFromUser(selectedUser, out userAndUid);
                }

                userAndUid ??= await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);

                if (userAndUid is { IsOversea: false })
                {
                    IHoyoPlayPassportClient passportClient = authScope.ServiceProvider
                        .GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>()
                        .CreateFor(userAndUid);
                    Response<AuthTicketWrapper> ticketResp = await passportClient
                        .CreateAuthTicketAsync(userAndUid.User)
                        .ConfigureAwait(false);
                    if (ResponseValidator.TryValidate(ticketResp, authScope.ServiceProvider, out AuthTicketWrapper? wrapper))
                    {
                        string ticketArg = $"login_auth_ticket={wrapper.Ticket}";
                        cmdArgs = string.IsNullOrEmpty(cmdArgs) ? ticketArg : $"{cmdArgs} {ticketArg}";
                    }
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        string fullCmdLine = string.IsNullOrEmpty(cmdArgs)
            ? $"\"{gameFilePath}\""
            : $"\"{gameFilePath}\" {cmdArgs}";

        NativeMethods.STARTUPINFOW si = default;
        si.cb = (uint)Unsafe.SizeOf<NativeMethods.STARTUPINFOW>();

        if (!NativeMethods.CreateProcessW(
            gameFilePath, fullCmdLine, nint.Zero, nint.Zero, false,
            NativeMethods.CREATE_SUSPENDED, nint.Zero, gameDirectory,
            ref si, out NativeMethods.PROCESS_INFORMATION pi))
        {
            throw new InvalidOperationException($"CreateProcessW failed (error: {Marshal.GetLastWin32Error()})");
        }

        using YaeGameProcess process = new(pi.hProcess, pi.hThread, pi.dwProcessId);

        await using (YaeNamedPipeServer server = new(serviceProvider, process, config))
        {
            if (!InjectDll(pi.hProcess, pi.dwProcessId, dllPath))
            {
                process.Kill();
                throw new InvalidOperationException("DLL injection failed");
            }

            return await server.GetDataArrayAsync().ConfigureAwait(false);
        }
    }

    private static uint GetGameHash(string exePath)
    {
        Span<byte> buffer = stackalloc byte[0x10000];
        using FileStream stream = File.OpenRead(exePath);
        int bytesRead = stream.ReadAtLeast(buffer, 0x10000, throwOnEndOfStream: false);
        return Crc32.HashToUInt32(buffer[..bytesRead]);
    }

    private static unsafe bool InjectDll(nint hProcess, uint processId, string dllPath)
    {
        byte[] dllPathBytes = Encoding.Unicode.GetBytes(dllPath + "\0");

        nint remoteMem = NativeMethods.VirtualAllocEx(hProcess, nint.Zero, (nuint)dllPathBytes.Length, 0x3000, 0x04);
        if (remoteMem == nint.Zero)
        {
            return false;
        }

        if (!NativeMethods.WriteProcessMemory(hProcess, remoteMem, dllPathBytes, (nuint)dllPathBytes.Length, out _))
        {
            NativeMethods.VirtualFreeEx(hProcess, remoteMem, 0, 0x8000);
            return false;
        }

        nint kernel32 = NativeMethods.GetModuleHandleW("kernel32.dll");
        nint loadLibraryW = NativeMethods.GetProcAddress(kernel32, "LoadLibraryW");

        if (loadLibraryW == nint.Zero)
        {
            NativeMethods.VirtualFreeEx(hProcess, remoteMem, 0, 0x8000);
            return false;
        }

        nint hThread = NativeMethods.CreateRemoteThread(hProcess, nint.Zero, 0, loadLibraryW, remoteMem, 0, out _);

        if (hThread == nint.Zero)
        {
            NativeMethods.VirtualFreeEx(hProcess, remoteMem, 0, 0x8000);
            return false;
        }

        _ = NativeMethods.WaitForSingleObject(hThread, 10000);
        NativeMethods.CloseHandle(hThread);
        NativeMethods.VirtualFreeEx(hProcess, remoteMem, 0, 0x8000);

        nint remoteBase = FindModuleBase(processId, dllPath);
        if (remoteBase == nint.Zero)
        {
            return false;
        }

        nint localDll = NativeMethods.LoadLibraryExW(dllPath, nint.Zero, 0x01);
        if (localDll == nint.Zero)
        {
            return false;
        }

        nint localYaeMain = NativeMethods.GetProcAddress(localDll, "YaeMain");
        NativeMethods.FreeLibrary(localDll);

        if (localYaeMain == nint.Zero)
        {
            return false;
        }

        nint yaeMainRva = localYaeMain - localDll;
        nint remoteYaeMain = remoteBase + yaeMainRva;

        nint hThread2 = NativeMethods.CreateRemoteThread(hProcess, nint.Zero, 0, remoteYaeMain, nint.Zero, 0, out _);
        if (hThread2 == nint.Zero)
        {
            return false;
        }

        NativeMethods.CloseHandle(hThread2);
        return true;
    }

    private static unsafe nint FindModuleBase(uint processId, string modulePath)
    {
        const uint TH32CS_SNAPMODULE = 0x08;
        nint hSnap;

        do
        {
            hSnap = NativeMethods.CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, processId);
        }
        while (hSnap == -1 && Marshal.GetLastPInvokeError() == 24);

        if (hSnap == -1)
        {
            return nint.Zero;
        }

        NativeMethods.MODULEENTRY32W me = default;
        me.dwSize = (uint)sizeof(NativeMethods.MODULEENTRY32W);

        if (NativeMethods.Module32FirstW(hSnap, ref me))
        {
            do
            {
                string exePath = new(me.szExePath);
                if (string.Equals(exePath, modulePath, StringComparison.OrdinalIgnoreCase))
                {
                    NativeMethods.CloseHandle(hSnap);
                    return me.modBaseAddr;
                }
            }
            while (NativeMethods.Module32NextW(hSnap, ref me));
        }

        NativeMethods.CloseHandle(hSnap);
        return nint.Zero;
    }

    private sealed class YaeGameProcess : IProcess
    {
        private readonly nint hProcess;
        private readonly nint hThread;
        private readonly uint processId;
        private bool disposed;

        public YaeGameProcess(nint hProcess, nint hThread, uint processId)
        {
            this.hProcess = hProcess;
            this.hThread = hThread;
            this.processId = processId;
        }

        public int Id => (int)processId;

        public nint Handle => hProcess;

        public HWND MainWindowHandle => default;

        public bool HasExited => NativeMethods.WaitForSingleObject(hProcess, 0) == 0;

        public int ExitCode => 0;

        public void Start() { }

        public void ResumeMainThread()
        {
            _ = NativeMethods.ResumeThread(hThread);
        }

        public void WaitForExit()
        {
            _ = NativeMethods.WaitForSingleObject(hProcess, 0xFFFFFFFF);
        }

        public void Kill()
        {
            NativeMethods.TerminateProcess(hProcess, 1);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                NativeMethods.CloseHandle(hThread);
                NativeMethods.CloseHandle(hProcess);
                disposed = true;
            }
        }
    }

    private static partial class NativeMethods
    {
        public const uint CREATE_SUSPENDED = 0x4;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFOW
        {
            public uint cb;
            public nint lpReserved, lpDesktop, lpTitle;
            public uint dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars, dwFillAttribute, dwFlags;
            public ushort wShowWindow, cbReserved2;
            public nint lpReserved2, hStdInput, hStdOutput, hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public nint hProcess, hThread;
            public uint dwProcessId, dwThreadId;
        }

        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CreateProcessW(
            string? lpApplicationName, string lpCommandLine,
            nint lpProcessAttributes, nint lpThreadAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
            uint dwCreationFlags, nint lpEnvironment, string? lpCurrentDirectory,
            ref STARTUPINFOW lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [LibraryImport("kernel32.dll")]
        public static partial uint ResumeThread(nint hThread);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool TerminateProcess(nint hProcess, uint uExitCode);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CloseHandle(nint hObject);

        [LibraryImport("kernel32.dll")]
        public static partial nint VirtualAllocEx(nint hProcess, nint lpAddress, nuint dwSize, uint flAllocationType, uint flProtect);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool VirtualFreeEx(nint hProcess, nint lpAddress, nuint dwSize, uint dwFreeType);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, nuint nSize, out nuint lpNumberOfBytesWritten);

        [LibraryImport("kernel32.dll")]
        public static partial nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, nuint dwStackSize, nint lpStartAddress, nint lpParameter, uint dwCreationFlags, out uint lpThreadId);

        [LibraryImport("kernel32.dll")]
        public static partial uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial nint GetModuleHandleW(string lpModuleName);

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf8)]
        public static partial nint GetProcAddress(nint hModule, string lpProcName);

        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial nint LoadLibraryExW(string lpLibFileName, nint hFile, uint dwFlags);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool FreeLibrary(nint hLibModule);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        public static partial nint CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Module32FirstW(nint hSnapshot, ref MODULEENTRY32W lpme);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool Module32NextW(nint hSnapshot, ref MODULEENTRY32W lpme);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct MODULEENTRY32W
        {
            public uint dwSize;
            public uint th32ModuleID;
            public uint th32ProcessID;
            public uint GlSnapCnt;
            public uint GlSnapUsage;
            public nint modBaseAddr;
            public uint modBaseSize;
            public nint hModule;
            public fixed char szModule[256];
            public fixed char szExePath[260];
        }
    }
}
