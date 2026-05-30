//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using kyxsan.Core;
using kyxsan.Core.Property;
using kyxsan.Core.Setting;
using kyxsan.Model;
using kyxsan.Model.Intrinsic;
using kyxsan.Service.Abstraction;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.PathAbstraction;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using System.Text.RegularExpressions;

namespace kyxsan.Service.Game;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchOptions : DbStoreOptions, IRestrictedGamePathAccess
{
    private static readonly Regex ResolutionRegex = new(@"(\d+)x(\d+)", RegexOptions.Compiled);

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial LaunchOptions(IServiceProvider serviceProvider);

    [field: MaybeNull]
    public static IObservableProperty<bool> IsGameRunning { get => field ??= GameLifeCycle.IsGameRunningProperty; }

    [field: MaybeNull]
    public static IReadOnlyObservableProperty<bool> CanKillGameProcess { get => field ??= Property.Observe(IsGameRunning, value => kyxsanRuntime.IsProcessElevated && value); }

    public AsyncReaderWriterLock GamePathLock { get; } = new();

    [field: MaybeNull]
    public IObservableProperty<GamePathEntry?> GamePathEntry { get => field ??= CreateProperty(SettingKeys.LaunchGamePath, string.Empty).AsNullableSelection(GamePathEntries.Value, static entry => entry?.Path ?? string.Empty, StringComparer.OrdinalIgnoreCase).Debug("GamePathEntry"); }

    [field: MaybeNull]
    public IObservableProperty<ImmutableArray<GamePathEntry>> GamePathEntries { get => field ??= CreatePropertyForStructUsingJson(SettingKeys.LaunchGamePathEntries, ImmutableArray<GamePathEntry>.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingHoyolabAccount { get => field ??= CreateProperty(SettingKeys.LaunchUsingHoyolabAccount, false); }

    [field: MaybeNull]
    public IObservableProperty<string> SelectedHoyolabUserMid { get => field ??= CreateProperty(SettingKeys.LaunchSelectedHoyolabUserMid, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> AreCommandLineArgumentsEnabled { get => field ??= CreateProperty(SettingKeys.LaunchAreCommandLineArgumentsEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsFullScreen { get => field ??= CreateProperty(SettingKeys.LaunchIsFullScreen, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsBorderless { get => field ??= CreateProperty(SettingKeys.LaunchIsBorderless, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsWindowed { get => field ??= CreateProperty(SettingKeys.LaunchIsWindowed, false); }

    [field: MaybeNull]
    public IObservableProperty<int> ResolutionPresetIndex { get => field ??= CreateProperty(SettingKeys.LaunchResolutionPresetIndex, -1); }

    private static ImmutableArray<NameValue<int>>? _commonResolutions;
    public static ImmutableArray<NameValue<int>> CommonResolutions => _commonResolutions ??= InitializeCommonResolutions();

    private static ImmutableArray<NameValue<int>> InitializeCommonResolutions()
    {
        ImmutableArray<NameValue<int>>.Builder builder = ImmutableArray.CreateBuilder<NameValue<int>>();
        builder.Add(new NameValue<int>("1280x720 (720p HD)", 1280));
        builder.Add(new NameValue<int>("1366x768 (WXGA)", 1366));
        builder.Add(new NameValue<int>("1600x900 (HD+)", 1600));
        builder.Add(new NameValue<int>("1920x1080 (1080p FHD)", 1920));
        builder.Add(new NameValue<int>("2560x1440 (2K QHD)", 2560));
        builder.Add(new NameValue<int>("3840x2160 (4K UHD)", 3840));
        builder.Add(new NameValue<int>("1280x800 (WXGA)", 1280));
        builder.Add(new NameValue<int>("1440x900 (WXGA+)", 1440));
        builder.Add(new NameValue<int>("1680x1050 (WSXGA+)", 1680));
        builder.Add(new NameValue<int>("1920x1200 (WUXGA)", 1920));
        builder.Add(new NameValue<int>("2560x1600 (WQXGA)", 2560));
        builder.Add(new NameValue<int>("1024x768 (XGA)", 1024));
        builder.Add(new NameValue<int>("1280x960 (SXGA-)", 1280));
        builder.Add(new NameValue<int>("1400x1050 (SXGA+)", 1400));
        builder.Add(new NameValue<int>("1600x1200 (UXGA)", 1600));
        builder.Add(new NameValue<int>("1920x1440 (SXGA)", 1920));
        builder.Add(new NameValue<int>("2560x1080 (UWFHD 21:9)", 2560));
        builder.Add(new NameValue<int>("3440x1440 (UWQHD 21:9)", 3440));
        builder.Add(new NameValue<int>("5120x1440 (DUHD 32:9)", 5120));
        return builder.ToImmutable();
    }

    public void ApplyResolutionPreset()
    {
        int index = ResolutionPresetIndex.Value;
        if (index < 0 || index >= CommonResolutions.Length)
        {
            return;
        }

        Match match = ResolutionRegex.Match(CommonResolutions[index].Name);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int width) && int.TryParse(match.Groups[2].Value, out int height))
        {
            ScreenWidth.Value = width;
            ScreenHeight.Value = height;
            IsScreenWidthEnabled.Value = true;
            IsScreenHeightEnabled.Value = true;
        }
    }

    public void SetWindowed(bool isOn)
    {
        if (isOn)
        {
            IsBorderless.Value = false;
            IsFullScreen.Value = false;
        }
    }

    public void SetBorderless(bool isOn)
    {
        if (isOn)
        {
            IsWindowed.Value = false;
            IsFullScreen.Value = false;
        }
    }

    public void SetFullScreen(bool isOn)
    {
        if (isOn)
        {
            IsWindowed.Value = false;
            IsBorderless.Value = false;
        }
    }

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int GetPrivateProfileInt(string section, string key, int defaultValue, string filePath);

    public void LoadGameResolutionFromConfig(string gameDirectory)
    {
        RefreshFromRegistry();
    }

    public void RefreshFromRegistry()
    {
        try
        {
            string[] registryPaths =
            [
                @"SOFTWARE\miHoYo\原神",
                @"SOFTWARE\miHoYo\Genshin Impact",
                @"SOFTWARE\WOW6432Node\miHoYo\原神",
                @"SOFTWARE\WOW6432Node\miHoYo\Genshin Impact",
                @"Software\miHoYo\原神",
                @"Software\miHoYo\Genshin Impact",
            ];

            foreach (string path in registryPaths)
            {
                using Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
                if (key is null)
                {
                    continue;
                }

                string? widthName = null;
                string? heightName = null;

                foreach (string name in key.GetValueNames())
                {
                    if (name.Contains("Width") && !name.Contains("Graphics"))
                    {
                        widthName = name;
                    }
                    else if (name.Contains("Height") && !name.Contains("Graphics"))
                    {
                        heightName = name;
                    }
                }

                if (widthName is null || heightName is null)
                {
                    continue;
                }

                object? widthObj = key.GetValue(widthName);
                object? heightObj = key.GetValue(heightName);

                if (widthObj is int width && heightObj is int height && width > 0 && height > 0 && width < 10000 && height < 10000)
                {
                    ScreenWidth.Value = width;
                    ScreenHeight.Value = height;
                    IsScreenWidthEnabled.Value = true;
                    IsScreenHeightEnabled.Value = true;

                    int matchedIndex = FindMatchingResolutionIndex(width, height);
                    ResolutionPresetIndex.Value = matchedIndex;
                }

                return;
            }
        }
        catch
        {
        }
    }

    private static int FindMatchingResolutionIndex(int width, int height)
    {
        for (int i = 0; i < CommonResolutions.Length; i++)
        {
            Match match = ResolutionRegex.Match(CommonResolutions[i].Name);
            if (match.Success
                && int.TryParse(match.Groups[1].Value, out int pw)
                && int.TryParse(match.Groups[2].Value, out int ph)
                && pw == width
                && ph == height)
            {
                return i;
            }
        }

        return -1;
    }

    public void WriteDisplaySettingsToRegistry(bool isOversea)
    {
        try
        {
            string[] registryPaths = isOversea
                ? [@"Software\miHoYo\Genshin Impact", @"SOFTWARE\miHoYo\Genshin Impact"]
                : [@"Software\miHoYo\原神", @"SOFTWARE\miHoYo\原神"];

            foreach (string path in registryPaths)
            {
                using Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, writable: true);
                if (key is null)
                {
                    continue;
                }

                foreach (string name in key.GetValueNames())
                {
                    if (name.Contains("Fullscreen"))
                    {
                        int mode = IsFullScreen.Value ? 1 : (IsBorderless.Value ? 2 : 0);
                        key.SetValue(name, mode, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                    else if (name.Contains("Width") && !name.Contains("Graphics") && IsScreenWidthEnabled.Value)
                    {
                        key.SetValue(name, ScreenWidth.Value, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                    else if (name.Contains("Height") && !name.Contains("Graphics") && IsScreenHeightEnabled.Value)
                    {
                        key.SetValue(name, ScreenHeight.Value, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                }

                return;
            }
        }
        catch
        {
        }
    }

    [field: MaybeNull]
    public IObservableProperty<bool> IsExclusive { get => field ??= CreateProperty(SettingKeys.LaunchIsExclusive, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsScreenWidthEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsScreenWidthEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> ScreenWidth { get => field ??= CreateProperty(SettingKeys.LaunchScreenWidth, DisplayArea.Primary.OuterBounds.Width); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsScreenHeightEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsScreenHeightEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> ScreenHeight { get => field ??= CreateProperty(SettingKeys.LaunchScreenHeight, DisplayArea.Primary.OuterBounds.Height); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsMonitorEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsMonitorEnabled, true); }

    public ImmutableArray<NameValue<int>> Monitors { get; } = InitializeMonitors();

    [field: MaybeNull]
    public IObservableProperty<NameValue<int>?> Monitor { get => field ??= CreatePropertyForSelectedOneBasedIndex(SettingKeys.LaunchMonitor, Monitors); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsPlatformTypeEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsPlatformTypeEnabled, false); }

    public ImmutableArray<NameValue<PlatformType>> PlatformTypes { get; } = ImmutableCollectionsNameValue.FromEnum<PlatformType>();

    [field: MaybeNull]
    public IObservableProperty<PlatformType> PlatformType { get => field ??= CreateProperty(SettingKeys.LaunchPlatformType, Model.Intrinsic.PlatformType.PC); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsWindowsHDREnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsWindowsHDREnabled, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingStarwardPlayTimeStatistics { get => field ??= CreateProperty(SettingKeys.LaunchUsingStarwardPlayTimeStatistics, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingBetterGenshinImpactAutomation { get => field ??= CreateProperty(SettingKeys.LaunchUsingBetterGenshinImpactAutomation, false); }

    [field: MaybeNull]
    public IObservableProperty<string> BgiPath { get => field ??= CreateProperty(SettingKeys.LaunchBgiPath, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<int> BgiDelay { get => field ??= CreateProperty(SettingKeys.LaunchBgiDelay, 5); }

    [field: MaybeNull]
    public IObservableProperty<string> BgiArgs { get => field ??= CreateProperty(SettingKeys.LaunchBgiArgs, "start"); }

    [field: MaybeNull]
    public IObservableProperty<bool> AttachProgramEnabled { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgramEnabled, false); }

    [field: MaybeNull]
    public IObservableProperty<string> AttachProgramPath { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgramPath, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<int> AttachProgramDelay { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgramDelay, 3); }

    [field: MaybeNull]
    public IObservableProperty<string> AttachProgramArgs { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgramArgs, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> AttachProgram2Enabled { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram2Enabled, false); }

    [field: MaybeNull]
    public IObservableProperty<string> AttachProgram2Path { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram2Path, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<int> AttachProgram2Delay { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram2Delay, 0); }

    [field: MaybeNull]
    public IObservableProperty<string> AttachProgram2Args { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram2Args, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> AttachProgram3Enabled { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram3Enabled, false); }

    [field: MaybeNull]
    public IObservableProperty<string> AttachProgram3Path { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram3Path, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<int> AttachProgram3Delay { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram3Delay, 0); }

    [field: MaybeNull]
    public IObservableProperty<string> AttachProgram3Args { get => field ??= CreateProperty(SettingKeys.LaunchAttachProgram3Args, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsIslandEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsIslandEnabled, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsIslandRiskAccepted { get => field ??= CreateProperty(SettingKeys.LaunchIsIslandRiskAccepted, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsSetFieldOfViewEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsSetFieldOfViewEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<float> TargetFov { get => field ??= CreateProperty(SettingKeys.LaunchTargetFov, 45f); }

    [field: MaybeNull]
    public IObservableProperty<bool> FixLowFovScene { get => field ??= CreateProperty(SettingKeys.LaunchFixLowFovScene, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableFog { get => field ??= CreateProperty(SettingKeys.LaunchDisableFogRendering, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> RemoveOpenTeamProgress { get => field ??= CreateProperty(SettingKeys.LaunchRemoveOpenTeamProgress, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> HideQuestBanner { get => field ??= CreateProperty(SettingKeys.LaunchHideQuestBanner, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableEventCameraMove { get => field ??= CreateProperty(SettingKeys.LaunchDisableEventCameraMove, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableShowDamageText { get => field ??= CreateProperty(SettingKeys.LaunchDisableShowDamageText, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> UsingTouchScreen { get => field ??= CreateProperty(SettingKeys.LaunchUsingTouchScreen, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnablePortableCraftingBench { get => field ??= CreateProperty(SettingKeys.LaunchEnableCraftRedirect, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> RedirectCombineEntry { get => field ??= CreateProperty(SettingKeys.LaunchRedirectCombineEntry, false); }

    [field: MaybeNull]
    public IObservableProperty<int> CraftKey { get => field ??= CreateProperty(SettingKeys.LaunchCraftKey, 67); }

    [field: MaybeNull]
    public IObservableProperty<int> CraftModifier { get => field ??= CreateProperty(SettingKeys.LaunchCraftModifier, 1); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnableDispatch { get => field ??= CreateProperty(SettingKeys.LaunchEnableDispatch, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> RedirectDispatch { get => field ??= CreateProperty(SettingKeys.LaunchRedirectDispatch, false); }

    [field: MaybeNull]
    public IObservableProperty<int> DispatchKey { get => field ??= CreateProperty(SettingKeys.LaunchDispatchKey, 0x76); }

    [field: MaybeNull]
    public IObservableProperty<int> DispatchModifier { get => field ??= CreateProperty(SettingKeys.LaunchDispatchModifier, 0); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnableCooking { get => field ??= CreateProperty(SettingKeys.LaunchEnableCooking, false); }

    [field: MaybeNull]
    public IObservableProperty<int> CookingKey { get => field ??= CreateProperty(SettingKeys.LaunchCookingKey, 0x77); }

    [field: MaybeNull]
    public IObservableProperty<int> CookingModifier { get => field ??= CreateProperty(SettingKeys.LaunchCookingModifier, 0); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnableForge { get => field ??= CreateProperty(SettingKeys.LaunchEnableForge, false); }

    [field: MaybeNull]
    public IObservableProperty<int> ForgeKey { get => field ??= CreateProperty(SettingKeys.LaunchForgeKey, 0x78); }

    [field: MaybeNull]
    public IObservableProperty<int> ForgeModifier { get => field ??= CreateProperty(SettingKeys.LaunchForgeModifier, 0); }



    // Custom DLL Injection - 每个DLL单独控制启用状态
    [field: MaybeNull]
    public IObservableProperty<ImmutableDictionary<string, bool>> CustomDllConfigs { get => field ??= CreatePropertyForClassUsingCustom(SettingKeys.LaunchCustomDllConfigs, ImmutableDictionary<string, bool>.Empty, s => JsonSerializer.Deserialize<ImmutableDictionary<string, bool>>(s) ?? ImmutableDictionary<string, bool>.Empty, d => JsonSerializer.Serialize(d)); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableCharFade { get => field ??= CreateProperty(SettingKeys.LaunchDisableCharFade, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> HideUID { get => field ??= CreateProperty(SettingKeys.LaunchHideUID, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> HideMenuUID { get => field ??= CreateProperty(SettingKeys.LaunchHideMenuUID, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> DisableVSync { get => field ??= CreateProperty(SettingKeys.LaunchDisableVSync, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnableFps { get => field ??= CreateProperty(SettingKeys.LaunchEnableFps, false); }

    [field: MaybeNull]
    public IObservableProperty<int> TargetFps { get => field ??= CreateProperty(SettingKeys.LaunchTargetFps, 120); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId000106Allowed { get => field ??= CreateProperty(SettingKeys.LaunchResinListItemId000106Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId000201Allowed { get => field ??= CreateProperty(SettingKeys.LaunchResinListItemId000201Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId107009Allowed { get => field ??= CreateProperty(SettingKeys.LaunchResinListItemId107009Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId107012Allowed { get => field ??= CreateProperty(SettingKeys.LaunchResinListItemId107012Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> ResinListItemId220007Allowed { get => field ??= CreateProperty(SettingKeys.LaunchResinListItemId220007Allowed, true); }

    [field: MaybeNull]
    public IObservableProperty<ImmutableArray<AspectRatio>> AspectRatios { get => field ??= CreatePropertyForStructUsingJson(SettingKeys.LaunchAspectRatios, ImmutableArray<AspectRatio>.Empty); }

    public AspectRatio? SelectedAspectRatio
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                (ScreenWidth.Value, ScreenHeight.Value) = ((int)value.Width, (int)value.Height);
            }
        }
    }

    [field: MaybeNull]
    public IObservableProperty<string> AdvancedStartProgramPath { get => field ??= CreateProperty(SettingKeys.LaunchAdvancedStartProgramPath, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> AdvancedStartDelayedOnAdvancedStart { get => field ??= CreateProperty(SettingKeys.LaunchAdvancedStartDelayedOnAdvancedStart, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> AdvancedStartDelayedOnGameLaunch { get => field ??= CreateProperty(SettingKeys.LaunchAdvancedStartDelayedOnGameLaunch, false); }

    private static ImmutableArray<NameValue<int>> InitializeMonitors()
    {
        ImmutableArray<NameValue<int>>.Builder monitors = ImmutableArray.CreateBuilder<NameValue<int>>();
        try
        {
            // This list can't use foreach
            // https://github.com/microsoft/CsWinRT/issues/747
            IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
            for (int i = 0; i < displayAreas.Count; i++)
            {
                DisplayArea displayArea = displayAreas[i];
                int index = i + 1;
                monitors.Add(new($"{displayArea.DisplayId.Value:X8}:{index}", index));
            }
        }
        catch
        {
            monitors.Clear();
        }

        return monitors.ToImmutable();
    }
}