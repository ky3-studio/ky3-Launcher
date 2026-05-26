//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Diagnostics;
using kyxsan.Service.Game.Launching.Context;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.Service.Game.Launching.Handler;

internal sealed partial class LaunchExecutionAttachedProgramHandler : AbstractLaunchExecutionHandler
{
    private nint jobHandle;
    private readonly List<string> fallbackExeNames = [];

    public override async ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (!context.Process.IsRunning)
        {
            return;
        }

        jobHandle = NativeMethods.CreateJobObjectW(nint.Zero, null);
        if (jobHandle != nint.Zero)
        {
            JOBOBJECT_EXTENDED_LIMIT_INFORMATION info = default;
            info.BasicLimitInformation.LimitFlags = 0x2000;
            NativeMethods.SetInformationJobObject(
                jobHandle, 9,
                ref info,
                (uint)Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>());
        }

        if (context.LaunchOptions.UsingBetterGenshinImpactAutomation.Value)
        {
            await LaunchAttachedProgramAsync(
                context.LaunchOptions.BgiPath.Value,
                context.LaunchOptions.BgiDelay.Value,
                context.LaunchOptions.BgiArgs.Value).ConfigureAwait(false);
        }

        if (context.LaunchOptions.AttachProgramEnabled.Value)
        {
            await LaunchAttachedProgramAsync(
                context.LaunchOptions.AttachProgramPath.Value,
                context.LaunchOptions.AttachProgramDelay.Value,
                context.LaunchOptions.AttachProgramArgs.Value).ConfigureAwait(false);
        }

        if (context.LaunchOptions.AttachProgram2Enabled.Value)
        {
            await LaunchAttachedProgramAsync(
                context.LaunchOptions.AttachProgram2Path.Value,
                context.LaunchOptions.AttachProgram2Delay.Value,
                context.LaunchOptions.AttachProgram2Args.Value).ConfigureAwait(false);
        }

        if (context.LaunchOptions.AttachProgram3Enabled.Value)
        {
            await LaunchAttachedProgramAsync(
                context.LaunchOptions.AttachProgram3Path.Value,
                context.LaunchOptions.AttachProgram3Delay.Value,
                context.LaunchOptions.AttachProgram3Args.Value).ConfigureAwait(false);
        }
    }

    public override ValueTask AfterAsync(AfterLaunchExecutionContext context)
    {
        if (jobHandle != nint.Zero)
        {
            NativeMethods.TerminateJobObject(jobHandle, 0);
            NativeMethods.CloseHandle(jobHandle);
            jobHandle = nint.Zero;
        }

        foreach (string exeName in fallbackExeNames)
        {
            try
            {
                using Process? p = Process.Start(new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/F /T /IM \"{exeName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                });
                p?.WaitForExit(3000);
            }
            catch
            {
            }
        }

        fallbackExeNames.Clear();
        return ValueTask.CompletedTask;
    }

    private async ValueTask LaunchAttachedProgramAsync(string programPath, int delay, string args)
    {
        if (string.IsNullOrEmpty(programPath) || !File.Exists(programPath))
        {
            return;
        }

        if (delay > 0)
        {
            await Task.Delay(delay * 1000).ConfigureAwait(false);
        }

        string fileName = Path.GetFileName(programPath).ToLowerInvariant();
        if (string.IsNullOrEmpty(args) && (fileName.Contains("bettergi") || fileName.Contains("bettergenshin")))
        {
            args = "start";
        }

        bool assigned = false;
        try
        {
            Process? proc = Process.Start(new ProcessStartInfo
            {
                FileName = programPath,
                Arguments = args,
                WorkingDirectory = Path.GetDirectoryName(programPath),
                UseShellExecute = false,
            });

            if (proc is not null)
            {
                if (jobHandle != nint.Zero)
                {
                    assigned = NativeMethods.AssignProcessToJobObject(jobHandle, proc.Handle);
                }

                proc.Dispose();
            }
        }
        catch
        {
        }

        if (assigned)
        {
            return;
        }

        try
        {
            Process? proc = Process.Start(new ProcessStartInfo
            {
                FileName = programPath,
                Arguments = args,
                WorkingDirectory = Path.GetDirectoryName(programPath),
                UseShellExecute = true,
            });

            if (proc is not null)
            {
                if (jobHandle != nint.Zero)
                {
                    NativeMethods.AssignProcessToJobObject(jobHandle, proc.Handle);
                }

                proc.Dispose();
            }

            string exeName = Path.GetFileName(programPath);
            if (!string.IsNullOrEmpty(exeName) && !fallbackExeNames.Contains(exeName))
            {
                fallbackExeNames.Add(exeName);
            }
        }
        catch
        {
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public nuint MinimumWorkingSetSize;
        public nuint MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public nuint Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public nuint ProcessMemoryLimit;
        public nuint JobMemoryLimit;
        public nuint PeakProcessMemoryUsed;
        public nuint PeakJobMemoryUsed;
    }

    private static partial class NativeMethods
    {
        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial nint CreateJobObjectW(nint lpJobAttributes, string? lpName);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetInformationJobObject(
            nint hJob, int jobObjectInfoClass,
            ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInformation,
            uint cbJobObjectInformationLength);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AssignProcessToJobObject(nint hJob, nint hProcess);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool TerminateJobObject(nint hJob, uint uExitCode);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CloseHandle(nint hObject);
    }
}
