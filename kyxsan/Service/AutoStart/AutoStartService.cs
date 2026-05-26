// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using kyxsan.Core;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.Setting;
using kyxsan.Service.Notification;

namespace kyxsan.Service;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class AutoStartService
{
    private const string TaskName = "Snapkyxsan AutoStart";

    public AutoStartService(IServiceProvider serviceProvider) { }

    public bool IsStartupEnabled()
    {
        try
        {
            return LocalSetting.Get(SettingKeys.StartupEnabled, false);
        }
        catch
        {
            return false;
        }
    }

    public bool IsRunElevatedEnabled()
    {
        try
        {
            return LocalSetting.Get(SettingKeys.RunElevated, false);
        }
        catch
        {
            return false;
        }
    }

    public void EnsureUpToDate()
    {
        try
        {
            bool enable = IsStartupEnabled();
            bool expectedRunElevated = IsRunElevatedEnabled();

            if (!enable)
            {
                return;
            }

            if (!TryUseNativeHelper(out _))
            {
                return;
            }

            bool taskActive;
            try
            {
                taskActive = NativeMethods.is_auto_start_task_active_for_this_user();
            }
            catch
            {
                taskActive = false;
            }

            bool taskExecutableValid;
            bool taskExecutableMatchesCurrent;
            try
            {
                taskExecutableValid = AutoStartService.TryGetAutoStartTaskExecutablePath(out string? taskExePath)
                    && !string.IsNullOrWhiteSpace(taskExePath)
                    && File.Exists(taskExePath);

                taskExecutableMatchesCurrent = taskExecutableValid && IsExecutablePathMatchCurrent(taskExePath!);
            }
            catch
            {
                taskExecutableValid = false;
                taskExecutableMatchesCurrent = false;
            }

            if (!taskActive)
            {
                if (!kyxsanRuntime.IsProcessElevated)
                {
                    return;
                }

                SetStartup(true, expectedRunElevated);
                return;
            }

            bool taskElevatedMatches;
            try
            {
                bool taskRunElevated = NativeMethods.is_auto_start_task_run_elevated_for_this_user();
                taskElevatedMatches = taskRunElevated == expectedRunElevated;
            }
            catch
            {
                taskElevatedMatches = false;
            }

            if (!taskExecutableValid || !taskExecutableMatchesCurrent || !taskElevatedMatches)
            {
                if (!kyxsanRuntime.IsProcessElevated)
                {
                    return;
                }

                SetStartup(true, expectedRunElevated);
            }
        }
        catch
        {
        }
    }

    private static bool IsExecutablePathMatchCurrent(string taskExePath)
    {
        string currentPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
        if (string.IsNullOrEmpty(currentPath) || string.IsNullOrEmpty(taskExePath))
        {
            return false;
        }

        try
        {
            return string.Equals(Path.GetFullPath(taskExePath), Path.GetFullPath(currentPath), StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return string.Equals(taskExePath, currentPath, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static bool TryGetAutoStartTaskExecutablePath(out string? taskPath)
    {
        taskPath = null;

        char[] buffer = ArrayPool<char>.Shared.Rent(1024);
        try
        {
            if (!NativeMethods.TryGetAutoStartTaskExecutablePath(buffer))
            {
                return false;
            }

            int idx = Array.IndexOf(buffer, '\0');
            if (idx < 0)
            {
                idx = buffer.Length;
            }

            taskPath = new(buffer, 0, idx);
            return true;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    public void EnsureValidityAsync() => EnsureUpToDate();

    public void SyncAutoStartPrivilegeSettings()
    {
        try
        {
            if (!IsStartupEnabled())
            {
                return;
            }

            EnsureUpToDate();
        }
        catch
        {
        }
    }

    public void SetStartup(bool enable, bool runElevated)
    {
        try
        {
            if (enable)
            {
                RegisterAutoStartTask(runElevated);
            }
            else
            {
                UnregisterAutoStartTask();
            }
            LocalSetting.Set(SettingKeys.StartupEnabled, enable);
            LocalSetting.Set(SettingKeys.RunElevated, runElevated);
        }
        catch (Exception ex)
        {
            try { SentrySdk.CaptureException(ex); } catch { }
            throw;
        }
    }

    public bool IsExecutablePathValid()
    {
        try
        {
            if (!IsStartupEnabled())
            {
                return true;
            }

            if (!TryUseNativeHelper(out _))
            {
                return true;
            }

            return NativeMethods.is_auto_start_task_active_for_this_user();
        }
        catch
        {
            return false;
        }
    }

    private static bool TryUseNativeHelper(out string? reason)
    {
        reason = null;
        try
        {
            string exeDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty) ?? string.Empty;
            string helperPath = Path.Combine(exeDir, "Runner.dll");
            if (!File.Exists(helperPath))
            {
                reason = "Runner.dll not found.";
                IMessenger messenger = Ioc.Default.GetRequiredService<IMessenger>();
                messenger.Send(InfoBarMessage.Error("AutoStart feature is unavailable because Runner.dll is missing."));
                return false;
            }

            IntPtr h = NativeMethods.LoadLibrary(helperPath);
            if (h == IntPtr.Zero)
            {
                reason = $"LoadLibrary failed: {Marshal.GetLastWin32Error()}";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            reason = ex.Message;
            return false;
        }
    }

    private void RegisterAutoStartTask(bool runElevated)
    {
        string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
        if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
        {
            throw kyxsanException.InvalidOperation("Cannot find executable path to register autostart task.");
        }

        if (TryUseNativeHelper(out _))
        {
            try
            {
                _ = NativeMethods.delete_auto_start_task_for_this_user();

                bool ok = NativeMethods.create_auto_start_task_for_this_user(runElevated ? 1 : 0);
                if (ok)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                try { SentrySdk.CaptureException(ex); } catch { }
            }
        }
    }

    private void UnregisterAutoStartTask()
    {
        if (TryUseNativeHelper(out _))
        {
            try
            {
                bool ok = NativeMethods.delete_auto_start_task_for_this_user();
                if (ok)
                {
                    System.Diagnostics.ProcessStartInfo psi = new()
                    {
                        FileName = "schtasks.exe",
                        Arguments = $"/Delete /TN \"{TaskName}\" /F",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };

                    using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi)!)
                    {
                        proc.WaitForExit();
                    }

                    return;
                }
            }
            catch
            {
            }
        }
    }

    static partial class NativeMethods
    {
        public static IntPtr LoadLibrary(string lpFileName)
        {
            try
            {
                return System.Runtime.InteropServices.NativeLibrary.Load(lpFileName);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public static bool FreeLibrary(IntPtr hModule)
        {
            if (hModule == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                System.Runtime.InteropServices.NativeLibrary.Free(hModule);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [LibraryImport("Runner.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool create_auto_start_task_for_this_user(int runElevated);

        [LibraryImport("Runner.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool delete_auto_start_task_for_this_user();

        [LibraryImport("Runner.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool is_auto_start_task_active_for_this_user();

        [LibraryImport("Runner.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool is_auto_start_task_run_elevated_for_this_user();

        [LibraryImport("Runner.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool get_auto_start_task_executable_path_for_this_user([Out] char[] buffer, uint cchBuffer);

        public static bool TryGetAutoStartTaskExecutablePath(char[] buffer)
        {
            return get_auto_start_task_executable_path_for_this_user(buffer, (uint)buffer.Length);
        }
    }
}
