//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Core.Diagnostics;
using Launcher.Core.IO;
using Launcher.Factory.Process;
using Launcher.Service.Game.FileSystem;
using Launcher.Service.Game.Launching.Context;
using Launcher.Win32.Foundation;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Launcher.Service.Game.Launching;

internal sealed partial class GameProcessFactory
{
    private const string DllName = "Ky3-luancher-Plugin.dll";
    private const string DllConfigName = "dll_config.ini";

    public static IProcess CreateForDefault(BeforeLaunchExecutionContext context)
    {
        LaunchOptions launchOptions = context.LaunchOptions;

        string commandLine = string.Empty;

        string? authTicket = default;
        bool useAuthTicket = launchOptions.UsingHoyolabAccount.Value
            && context.TryGetOption(LaunchExecutionOptionsKey.LoginAuthTicket, out authTicket)
            && !string.IsNullOrEmpty(authTicket);

        if (launchOptions.AreCommandLineArgumentsEnabled.Value)
        {
            commandLine = new CommandLineBuilder()
                .AppendIf(launchOptions.IsWindowed.Value, "-windowed")
                .AppendIf(launchOptions.IsBorderless.Value, "-popupwindow")
                .AppendIf(launchOptions.IsExclusive.Value, "-window-mode", "exclusive")
                .Append("-screen-fullscreen", launchOptions.IsFullScreen.Value ? "1" : "0")
                .AppendIf(launchOptions.IsScreenWidthEnabled.Value, "-screen-width", launchOptions.ScreenWidth.Value)
                .AppendIf(launchOptions.IsScreenHeightEnabled.Value, "-screen-height", launchOptions.ScreenHeight.Value)
                .AppendIf(launchOptions.IsMonitorEnabled.Value, "-monitor", launchOptions.Monitor.Value?.Value ?? 1)
                .AppendIf(launchOptions.IsPlatformTypeEnabled.Value, "-platform_type", $"{launchOptions.PlatformType.Value:G}")
                .AppendIf(useAuthTicket, "login_auth_ticket", authTicket, CommandLineArgumentPrefix.Equal)
                .ToString();

            context.TaskContext.InvokeOnMainThread(() =>
            {
                launchOptions.AspectRatios.Add(new(launchOptions.ScreenWidth.Value, launchOptions.ScreenHeight.Value));
            });
        }
        else if (useAuthTicket)
        {
            commandLine = new CommandLineBuilder()
                .Append("login_auth_ticket", authTicket, CommandLineArgumentPrefix.Equal)
                .ToString();
        }

        string gameFilePath = context.FileSystem.GameFilePath;
        string gameDirectory = context.FileSystem.GameDirectory;
        string LauncherDirectory = AppContext.BaseDirectory;
        string dllPath = Path.Combine(LauncherDirectory, DllName);

        launchOptions.WriteDisplaySettingsToRegistry(context.TargetScheme.IsOversea);

        bool islandEnabled = launchOptions.IsIslandEnabled.Value && File.Exists(dllPath);

        ImmutableArray<string> customDlls = launchOptions.CustomDllConfigs.Value
            .Where(kv => kv.Value && File.Exists(kv.Key))
            .Select(kv => kv.Key)
            .ToImmutableArray();

        if (islandEnabled || customDlls.Length > 0)
        {
            if (islandEnabled)
            {
                CreateDllConfig(launchOptions, LauncherDirectory);
            }

            return CreateWithInjection(commandLine, gameFilePath, gameDirectory, islandEnabled ? dllPath : string.Empty, customDlls);
        }

        return ProcessFactory.CreateUsingShellExecute(commandLine, gameFilePath, gameDirectory);
    }

    public static int RunElevatedInjection(string[] args)
    {
        try
        {
            if (args.Length < 2) return 1;

            string configFile = args[1];
            if (!File.Exists(configFile)) return 1;

            string[] lines = File.ReadAllLines(configFile);
            if (lines.Length < 5) return 1;

            string gameExe = lines[0];
            string injDllPath = lines[1];
            string workDir = lines[2];
            string cmdLineArgs = lines[3];

            // Parse custom DLLs
            int customDllCount = int.TryParse(lines[4], out int cnt) ? cnt : 0;
            List<string> customDlls = [];
            for (int i = 0; i < customDllCount && (5 + i) < lines.Length; i++)
            {
                string customPath = lines[5 + i];
                if (File.Exists(customPath))
                {
                    customDlls.Add(customPath);
                }
            }

            string fullCmdLine = string.IsNullOrEmpty(cmdLineArgs)
                ? $"\"{gameExe}\""
                : $"\"{gameExe}\" {cmdLineArgs}";

            STARTUPINFOW si = new();
#pragma warning disable CA1421
            si.cb = (uint)Marshal.SizeOf<STARTUPINFOW>();
#pragma warning restore CA1421

            bool created = NativeMethods.CreateProcessW(
                gameExe, fullCmdLine, nint.Zero, nint.Zero, false, 0x4, nint.Zero, workDir, ref si, out PROCESS_INFORMATION pi);

            if (!created) return 2;

            List<string> failures = [];

            if (!string.IsNullOrEmpty(injDllPath))
            {
                if (!InjectDll(pi.hProcess, injDllPath))
                {
                    NativeMethods.TerminateProcess(pi.hProcess, 1);
                    NativeMethods.CloseHandle(pi.hThread);
                    NativeMethods.CloseHandle(pi.hProcess);
                    return 3;
                }
            }

            foreach (string customDll in customDlls)
            {
                if (!InjectDll(pi.hProcess, customDll))
                {
                    failures.Add(Path.GetFileName(customDll));
                }
            }

            if (failures.Count > 0)
            {
                string msg = SH.ServiceGameLaunchingCustomDllInjectFailed + "\r\n\r\n" + string.Join("\r\n", failures);
                NativeMethods.MessageBoxW(nint.Zero, msg, "Ky3 Launcher", 0x00000030u);
            }

            _ = NativeMethods.ResumeThread(pi.hThread);
            NativeMethods.CloseHandle(pi.hThread);

            File.WriteAllText(configFile, pi.dwProcessId.ToString());

            NativeMethods.CloseHandle(pi.hProcess);
            return 0;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return 99;
        }
    }

    private static IProcess CreateWithInjection(string arguments, string fileName, string workingDirectory, string dllPath, ImmutableArray<string> customDlls)
    {
        string configFile = Path.Combine(Path.GetTempPath(), $"Launcher_inject_{Guid.NewGuid():N}.tmp");

        // Write config: gameExe, mainDll, workDir, cmdLine, then custom DLLs count and paths
        List<string> configLines = [fileName, dllPath, workingDirectory, arguments ?? string.Empty];
        configLines.Add(customDlls.Length.ToString());
        configLines.AddRange(customDlls);
        File.WriteAllLines(configFile, configLines);

        string currentExe = Environment.ProcessPath ?? throw new InvalidOperationException(SH.ServiceGameLaunchingCannotGetLauncherPath);

        System.Diagnostics.ProcessStartInfo psi = new()
        {
            FileName = currentExe,
            Arguments = $"--elevated-inject \"{configFile}\"",
            UseShellExecute = true,
            Verb = "runas",
            WorkingDirectory = Path.GetDirectoryName(currentExe)
        };

        System.Diagnostics.Process? helper;
        try
        {
            helper = System.Diagnostics.Process.Start(psi);
        }
        catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            FileOperationSafe.TryDelete(configFile);
            throw new InvalidOperationException(SH.ServiceGameLaunchingUserCancelledElevation);
        }

        if (helper is null)
        {
            FileOperationSafe.TryDelete(configFile);
            throw new InvalidOperationException(SH.ServiceGameLaunchingStartInjectionProcessFailed);
        }

        using (helper)
        {
            helper.WaitForExit();

            if (helper.ExitCode != 0)
            {
                FileOperationSafe.TryDelete(configFile);
                string reason = helper.ExitCode switch
                {
                    1 => SH.ServiceGameLaunchingInjectionReasonInvalidArgs,
                    2 => SH.ServiceGameLaunchingInjectionReasonCreateProcessFailed,
                    3 => SH.ServiceGameLaunchingInjectionReasonDllInjectFailed,
                    _ => SH.FormatServiceGameLaunchingInjectionReasonUnknown(helper.ExitCode)
                };
                throw new InvalidOperationException(SH.FormatServiceGameLaunchingInjectionFailed(reason));
            }
        }

        if (!File.Exists(configFile))
            throw new InvalidOperationException(SH.ServiceGameLaunchingCannotGetProcessInfo);

        string pidStr = File.ReadAllText(configFile).Trim();
        FileOperationSafe.TryDelete(configFile);

        if (!uint.TryParse(pidStr, out uint pid))
            throw new InvalidOperationException(SH.ServiceGameLaunchingInvalidProcessId);

        nint handle = NativeMethods.OpenProcess(0x00101000, false, pid);
        if (handle == nint.Zero)
            throw new InvalidOperationException(SH.ServiceGameLaunchingCannotConnectToProcess);

        return new InjectedProcess(handle, pid);
    }

    private static bool InjectDll(nint hProcess, string dllPath)
    {
        byte[] dllPathBytes = Encoding.Unicode.GetBytes(dllPath + "\0");

        nint remoteMem = NativeMethods.VirtualAllocEx(hProcess, nint.Zero, (nuint)dllPathBytes.Length, 0x3000, 0x04);
        if (remoteMem == nint.Zero) return false;

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

        uint loadResult = 0;
        NativeMethods.GetExitCodeThread(hThread, out loadResult);

        NativeMethods.CloseHandle(hThread);
        NativeMethods.VirtualFreeEx(hProcess, remoteMem, 0, 0x8000);

        return loadResult != 0;
    }



    public static void CreateDllConfig(LaunchOptions options, string directory)
    {
        string configPath = Path.Combine(directory, DllConfigName);

        StringBuilder config = new();
        config.AppendLine("[Settings]");
        config.AppendLine($"targetFov={(int)options.TargetFov.Value}");
        config.AppendLine($"disableVSync={(options.DisableVSync.Value ? 1 : 0)}");
        config.AppendLine($"enableFov={(options.IsSetFieldOfViewEnabled.Value ? 1 : 0)}");
        config.AppendLine($"disableFog={(options.DisableFog.Value ? 1 : 0)}");
        config.AppendLine($"disableCharFade={(options.DisableCharFade.Value ? 1 : 0)}");
        config.AppendLine($"hideUID={(options.HideUID.Value ? 1 : 0)}");
        config.AppendLine($"hideMenuUID={(options.HideMenuUID.Value ? 1 : 0)}");
        config.AppendLine($"hideQuestBanner={(options.HideQuestBanner.Value ? 1 : 0)}");
        config.AppendLine($"disableDamageText={(options.DisableShowDamageText.Value ? 1 : 0)}");
        config.AppendLine($"disableCameraAnim={(options.DisableEventCameraMove.Value ? 1 : 0)}");
        config.AppendLine($"touchScreen={(options.UsingTouchScreen.Value ? 1 : 0)}");
        config.AppendLine($"enablePortableCraft={(options.EnablePortableCraftingBench.Value ? 1 : 0)}");
        config.AppendLine($"redirectCraft={(options.RedirectCombineEntry.Value ? 1 : 0)}");
        config.AppendLine($"craftKey={options.CraftKey.Value}");
        config.AppendLine($"craftModifier={options.CraftModifier.Value}");
        config.AppendLine($"removeTeamAnim={(options.RemoveOpenTeamProgress.Value ? 1 : 0)}");
        config.AppendLine($"enableFps={(options.EnableFps.Value ? 1 : 0)}");
        config.AppendLine($"targetFps={options.TargetFps.Value}");
        config.AppendLine($"enableDispatch={(options.EnableDispatch.Value ? 1 : 0)}");
        config.AppendLine($"redirectDispatch={(options.RedirectDispatch.Value ? 1 : 0)}");
        config.AppendLine($"dispatchKey={options.DispatchKey.Value}");
        config.AppendLine($"dispatchModifier={options.DispatchModifier.Value}");
        config.AppendLine($"dispatchPageName=ExpeditionPage");
        config.AppendLine($"enableCooking={(options.EnableCooking.Value ? 1 : 0)}");
        config.AppendLine($"cookingKey={options.CookingKey.Value}");
        config.AppendLine($"cookingModifier={options.CookingModifier.Value}");
        config.AppendLine($"enableForge={(options.EnableForge.Value ? 1 : 0)}");
        config.AppendLine($"forgeKey={options.ForgeKey.Value}");
        config.AppendLine($"forgeModifier={options.ForgeModifier.Value}");
        config.AppendLine($"enableNoGrass={(options.EnableNoGrass.Value ? 1 : 0)}");
        config.AppendLine($"enableGui={(options.EnableGui.Value ? 1 : 0)}");
        config.AppendLine($"guiKey={options.GuiKey.Value}");
        config.AppendLine($"guiModifier={options.GuiModifier.Value}");
        config.AppendLine($"enableFreeCam={(options.EnableFreeCam.Value ? 1 : 0)}");
        config.AppendLine($"freeCamKey={options.FreeCamKey.Value}");
        config.AppendLine($"freeCamModifier={options.FreeCamModifier.Value}");
        config.AppendLine($"enableFreeCamLock={(options.EnableFreeCamLock.Value ? 1 : 0)}");
        config.AppendLine($"freeCamLockKey={options.FreeCamLockKey.Value}");
        config.AppendLine($"freeCamLockModifier={options.FreeCamLockModifier.Value}");
        config.AppendLine($"freeCamMoveSpeed={options.FreeCamMoveSpeed.Value.ToString(CultureInfo.InvariantCulture)}");
        config.AppendLine($"freeCamSprintMult={options.FreeCamSprintMult.Value.ToString(CultureInfo.InvariantCulture)}");
        config.AppendLine($"freeCamMouseSensitivity={options.FreeCamMouseSensitivity.Value.ToString(CultureInfo.InvariantCulture)}");
        config.AppendLine($"freeCamPitchLimit={options.FreeCamPitchLimit.Value.ToString(CultureInfo.InvariantCulture)}");

        FileOperationSafe.TryWriteAllText(configPath, config.ToString());
    }

    public static void UpdateDllConfigForHotSwitch(LaunchOptions options)
    {
        CreateDllConfig(options, AppContext.BaseDirectory);
    }

    public static void ReadDllConfigIntoOptions(LaunchOptions options)
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, DllConfigName);
        if (!File.Exists(configPath)) return;

        try
        {
            Dictionary<string, string> values = [];
            foreach (string line in File.ReadAllLines(configPath))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith('[') || string.IsNullOrEmpty(trimmed)) continue;
                int eq = trimmed.IndexOf('=');
                if (eq > 0)
                {
                    values[trimmed[..eq].Trim()] = trimmed[(eq + 1)..].Trim();
                }
            }

            static bool GetBool(Dictionary<string, string> v, string key, bool fallback)
                => v.TryGetValue(key, out string? s) && int.TryParse(s, out int i) ? i != 0 : fallback;
            static int GetInt(Dictionary<string, string> v, string key, int fallback)
                => v.TryGetValue(key, out string? s) && int.TryParse(s, out int i) ? i : fallback;
            static float GetFloat(Dictionary<string, string> v, string key, float fallback)
                => v.TryGetValue(key, out string? s) && float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : fallback;

            options.TargetFov.Value = GetInt(values, "targetFov", (int)options.TargetFov.Value);
            options.IsSetFieldOfViewEnabled.Value = GetBool(values, "enableFov", options.IsSetFieldOfViewEnabled.Value);
            options.DisableVSync.Value = GetBool(values, "disableVSync", options.DisableVSync.Value);
            options.DisableFog.Value = GetBool(values, "disableFog", options.DisableFog.Value);
            options.DisableCharFade.Value = GetBool(values, "disableCharFade", options.DisableCharFade.Value);
            options.HideUID.Value = GetBool(values, "hideUID", options.HideUID.Value);
            options.HideMenuUID.Value = GetBool(values, "hideMenuUID", options.HideMenuUID.Value);
            options.HideQuestBanner.Value = GetBool(values, "hideQuestBanner", options.HideQuestBanner.Value);
            options.DisableShowDamageText.Value = GetBool(values, "disableDamageText", options.DisableShowDamageText.Value);
            options.DisableEventCameraMove.Value = GetBool(values, "disableCameraAnim", options.DisableEventCameraMove.Value);
            options.UsingTouchScreen.Value = GetBool(values, "touchScreen", options.UsingTouchScreen.Value);
            options.RemoveOpenTeamProgress.Value = GetBool(values, "removeTeamAnim", options.RemoveOpenTeamProgress.Value);
            options.EnableFps.Value = GetBool(values, "enableFps", options.EnableFps.Value);
            options.TargetFps.Value = GetInt(values, "targetFps", options.TargetFps.Value);
            options.EnablePortableCraftingBench.Value = GetBool(values, "enablePortableCraft", options.EnablePortableCraftingBench.Value);
            options.RedirectCombineEntry.Value = GetBool(values, "redirectCraft", options.RedirectCombineEntry.Value);
            options.CraftKey.Value = GetInt(values, "craftKey", options.CraftKey.Value);
            options.CraftModifier.Value = GetInt(values, "craftModifier", options.CraftModifier.Value);
            options.EnableDispatch.Value = GetBool(values, "enableDispatch", options.EnableDispatch.Value);
            options.RedirectDispatch.Value = GetBool(values, "redirectDispatch", options.RedirectDispatch.Value);
            options.DispatchKey.Value = GetInt(values, "dispatchKey", options.DispatchKey.Value);
            options.DispatchModifier.Value = GetInt(values, "dispatchModifier", options.DispatchModifier.Value);
            options.EnableCooking.Value = GetBool(values, "enableCooking", options.EnableCooking.Value);
            options.CookingKey.Value = GetInt(values, "cookingKey", options.CookingKey.Value);
            options.CookingModifier.Value = GetInt(values, "cookingModifier", options.CookingModifier.Value);
            options.EnableForge.Value = GetBool(values, "enableForge", options.EnableForge.Value);
            options.ForgeKey.Value = GetInt(values, "forgeKey", options.ForgeKey.Value);
            options.ForgeModifier.Value = GetInt(values, "forgeModifier", options.ForgeModifier.Value);
            options.EnableNoGrass.Value = GetBool(values, "enableNoGrass", options.EnableNoGrass.Value);
            options.EnableGui.Value = GetBool(values, "enableGui", options.EnableGui.Value);
            options.GuiKey.Value = GetInt(values, "guiKey", options.GuiKey.Value);
            options.GuiModifier.Value = GetInt(values, "guiModifier", options.GuiModifier.Value);
            options.EnableFreeCam.Value = GetBool(values, "enableFreeCam", options.EnableFreeCam.Value);
            options.FreeCamKey.Value = GetInt(values, "freeCamKey", options.FreeCamKey.Value);
            options.FreeCamModifier.Value = GetInt(values, "freeCamModifier", options.FreeCamModifier.Value);
            options.EnableFreeCamLock.Value = GetBool(values, "enableFreeCamLock", options.EnableFreeCamLock.Value);
            options.FreeCamLockKey.Value = GetInt(values, "freeCamLockKey", options.FreeCamLockKey.Value);
            options.FreeCamLockModifier.Value = GetInt(values, "freeCamLockModifier", options.FreeCamLockModifier.Value);
            options.FreeCamMoveSpeed.Value = GetFloat(values, "freeCamMoveSpeed", options.FreeCamMoveSpeed.Value);
            options.FreeCamSprintMult.Value = GetFloat(values, "freeCamSprintMult", options.FreeCamSprintMult.Value);
            options.FreeCamMouseSensitivity.Value = GetFloat(values, "freeCamMouseSensitivity", options.FreeCamMouseSensitivity.Value);
            options.FreeCamPitchLimit.Value = GetFloat(values, "freeCamPitchLimit", options.FreeCamPitchLimit.Value);

        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    public static IProcess CreateForEmbeddedYae(BeforeLaunchExecutionContext context)
    {
        LaunchOptions launchOptions = context.LaunchOptions;

        string? authTicket = default;
        bool useAuthTicket = launchOptions.AreCommandLineArgumentsEnabled.Value
            && launchOptions.UsingHoyolabAccount.Value
            && context.TryGetOption(LaunchExecutionOptionsKey.LoginAuthTicket, out authTicket)
            && !string.IsNullOrEmpty(authTicket);

        string commandLine = new CommandLineBuilder()
            .Append("-screen-fullscreen", 0)
            .Append("-screen-width", 800)
            .Append("-screen-height", 450)
            .AppendIf(useAuthTicket, "login_auth_ticket", authTicket, CommandLineArgumentPrefix.Equal)
            .ToString();

        return ProcessFactory.CreateSuspended(commandLine, context.FileSystem.GameFilePath, context.FileSystem.GameDirectory);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct STARTUPINFOW
    {
        public uint cb;
        public nint lpReserved, lpDesktop, lpTitle;
        public uint dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars, dwFillAttribute, dwFlags;
        public ushort wShowWindow, cbReserved2;
        public nint lpReserved2, hStdInput, hStdOutput, hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public nint hProcess, hThread;
        public uint dwProcessId, dwThreadId;
    }

    private static partial class NativeMethods
    {
        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool CreateProcessW(string? lpApplicationName, string lpCommandLine, nint lpProcessAttributes, nint lpThreadAttributes, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, nint lpEnvironment, string? lpCurrentDirectory, ref STARTUPINFOW lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

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

        [LibraryImport("kernel32.dll", SetLastError = true)]
        public static partial nint OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetExitCodeThread(nint hThread, out uint lpExitCode);

        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int MessageBoxW(nint hWnd, string lpText, string lpCaption, uint uType);
    }

    private sealed class InjectedProcess : IProcess
    {
        private readonly nint _handle;
        private readonly uint _processId;
        private bool _disposed;

        public InjectedProcess(nint handle, uint processId) { _handle = handle; _processId = processId; }

        public int Id => (int)_processId;
        public nint Handle => _handle;
        public HWND MainWindowHandle => default;
        public bool HasExited => NativeMethods.WaitForSingleObject(_handle, 0) == 0;
        public int ExitCode => 0;
        public bool IsRunning => !HasExited;

        public void Start() { }
        public void ResumeMainThread() { }
        public void WaitForExit() => _ = NativeMethods.WaitForSingleObject(_handle, 0xFFFFFFFF);
        public void Kill() => NativeMethods.TerminateProcess(_handle, 1);

        public void Dispose()
        {
            if (!_disposed) { NativeMethods.CloseHandle(_handle); _disposed = true; }
        }
    }
}
