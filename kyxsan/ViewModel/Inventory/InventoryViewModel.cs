using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Service.Game;
using kyxsan.Service.Inventory;
using kyxsan.Service.Notification;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.ViewModel.Inventory;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class InventoryViewModel : Abstraction.ViewModel
{
    private readonly InventoryPacketService inventoryService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly LaunchOptions launchOptions;

    private uint gameProcessId;

    [GeneratedConstructor]
    public partial InventoryViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial ObservableCollection<WeaponEntry> Weapons { get; set; }

    [ObservableProperty]
    public partial List<WeaponEntry> FiveStarWeapons { get; set; }

    [ObservableProperty]
    public partial List<WeaponEntry> FourStarWeapons { get; set; }

    [ObservableProperty]
    public partial List<WeaponEntry> ThreeStarWeapons { get; set; }

    [ObservableProperty]
    public partial WeaponEntry? SelectedWeapon { get; set; }

    [ObservableProperty]
    public partial string StatusText { get; set; }

    [ObservableProperty]
    public partial int TotalWeaponCount { get; set; }

    [ObservableProperty]
    public partial int WeaponTypeCount { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool HasWeapons { get; set; }

    [ObservableProperty]
    public partial bool ShowDetail { get; set; }

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        Weapons = [];
        FiveStarWeapons = [];
        FourStarWeapons = [];
        ThreeStarWeapons = [];
        StatusText = SH.ViewInventoryStatusReady;
        return ValueTask.FromResult(true);
    }

    [Command("SelectWeaponCommand")]
    private void SelectWeapon(WeaponEntry? weapon)
    {
        if (weapon is null) return;
        SelectedWeapon = weapon;
        ShowDetail = true;
    }

    [Command("BackToListCommand")]
    private void BackToList()
    {
        ShowDetail = false;
        SelectedWeapon = null;
    }

    [Command("StartCaptureCommand")]
    private async Task StartCaptureAsync()
    {
        IsLoading = true;
        gameProcessId = 0;

        try
        {
            string dllPath = Path.Combine(AppContext.BaseDirectory, "PacketSniffer.dll");
            if (!File.Exists(dllPath))
            {
                StatusText = SH.ViewInventoryStatusNoDll;
                IsLoading = false;
                return;
            }

            string? gamePath = launchOptions.GamePathEntry?.Value?.Path;
            if (string.IsNullOrEmpty(gamePath) || !File.Exists(gamePath))
            {
                StatusText = "\u672a\u914d\u7f6e\u6e38\u620f\u8def\u5f84\uff0c\u8bf7\u5148\u5728\u542f\u52a8\u6e38\u620f\u9875\u9762\u8bbe\u7f6e";
                IsLoading = false;
                return;
            }

            string gameDir = Path.GetDirectoryName(gamePath)!;
            string jsonPath = Path.Combine(AppContext.BaseDirectory, "inventory.json");

            if (File.Exists(jsonPath))
                File.Delete(jsonPath);

            await taskContext.SwitchToMainThreadAsync();
            StatusText = "\u6b63\u5728\u542f\u52a8\u6e38\u620f\u5e76\u6ce8\u5165...";
            await taskContext.SwitchToBackgroundAsync();

            if (!LaunchGameWithDll(gamePath, gameDir, dllPath))
            {
                await taskContext.SwitchToMainThreadAsync();
                StatusText = SH.ViewInventoryStatusInjectFailed;
                IsLoading = false;
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            StatusText = SH.ViewInventoryStatusWaitingCapture;
            await taskContext.SwitchToBackgroundAsync();

            bool captured = await WaitForFileAsync(jsonPath, TimeSpan.FromSeconds(120)).ConfigureAwait(false);

            if (!captured)
            {
                await taskContext.SwitchToMainThreadAsync();
                StatusText = SH.ViewInventoryStatusNoData;
                KillGameProcess();
                IsLoading = false;
                return;
            }

            await Task.Delay(500).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            StatusText = SH.ViewInventoryStatusParsing;
            await taskContext.SwitchToBackgroundAsync();

            List<WeaponEntry> weapons = await inventoryService.LoadWeaponsFromJsonAsync(AppContext.BaseDirectory).ConfigureAwait(false);

            KillGameProcess();

            if (weapons.Count == 0)
            {
                await taskContext.SwitchToMainThreadAsync();
                StatusText = SH.ViewInventoryStatusNoData;
                IsLoading = false;
                return;
            }

            weapons.Sort((a, b) =>
            {
                int r = b.Rank.CompareTo(a.Rank);
                if (r != 0) return r;
                r = string.Compare(a.Type, b.Type, StringComparison.Ordinal);
                if (r != 0) return r;
                return b.Level.CompareTo(a.Level);
            });

            await taskContext.SwitchToMainThreadAsync();
            Weapons = new ObservableCollection<WeaponEntry>(weapons);
            FiveStarWeapons = weapons.Where(w => w.Rank >= 5).ToList();
            FourStarWeapons = weapons.Where(w => w.Rank == 4).ToList();
            ThreeStarWeapons = weapons.Where(w => w.Rank <= 3).ToList();
            TotalWeaponCount = weapons.Count;
            WeaponTypeCount = weapons.Select(w => w.Id).Distinct().Count();
            HasWeapons = true;
            StatusText = string.Format(SH.ViewInventoryStatusDone, TotalWeaponCount, WeaponTypeCount);
        }
        catch (Exception ex)
        {
            await taskContext.SwitchToMainThreadAsync();
            StatusText = $"{SH.ViewInventoryStatusParseFailed}: {ex.Message}";
            KillGameProcess();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool LaunchGameWithDll(string gamePath, string gameDir, string dllPath)
    {
        string configFile = Path.Combine(Path.GetTempPath(), $"kyxsan_inv_{Guid.NewGuid():N}.tmp");

        List<string> configLines = [gamePath, string.Empty, gameDir, string.Empty, "1", dllPath];
        File.WriteAllLines(configFile, configLines);

        string currentExe = Environment.ProcessPath ?? throw new InvalidOperationException();

        ProcessStartInfo psi = new()
        {
            FileName = currentExe,
            Arguments = $"--elevated-inject \"{configFile}\"",
            UseShellExecute = true,
            Verb = "runas",
            WorkingDirectory = Path.GetDirectoryName(currentExe)
        };

        Process? helper;
        try
        {
            helper = Process.Start(psi);
        }
        catch
        {
            try { File.Delete(configFile); } catch { }
            return false;
        }

        if (helper is null)
        {
            try { File.Delete(configFile); } catch { }
            return false;
        }

        using (helper)
        {
            helper.WaitForExit();
            if (helper.ExitCode != 0)
            {
                try { File.Delete(configFile); } catch { }
                return false;
            }
        }

        if (!File.Exists(configFile))
            return false;

        string pidStr = File.ReadAllText(configFile).Trim();
        try { File.Delete(configFile); } catch { }

        if (uint.TryParse(pidStr, out uint pid))
        {
            gameProcessId = pid;
            return true;
        }

        return false;
    }

    private void KillGameProcess()
    {
        if (gameProcessId == 0) return;

        try
        {
            nint handle = OpenProcess(0x0001, false, gameProcessId);
            if (handle != nint.Zero)
            {
                TerminateProcess(handle, 0);
                CloseHandle(handle);
            }
        }
        catch { }

        gameProcessId = 0;
    }

    private static async Task<bool> WaitForFileAsync(string path, TimeSpan timeout)
    {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (File.Exists(path))
            {
                try
                {
                    using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    return fs.Length > 10;
                }
                catch (IOException) { }
            }

            await Task.Delay(1000).ConfigureAwait(false);
        }

        return false;
    }

    [LibraryImport("kernel32.dll")]
    private static partial nint OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TerminateProcess(nint hProcess, uint uExitCode);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint hObject);
}
