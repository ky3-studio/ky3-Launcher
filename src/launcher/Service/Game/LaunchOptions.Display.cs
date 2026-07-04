//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Launcher.Core.Property;
using Launcher.Core.Setting;
using Launcher.Model;
using Launcher.Model.Intrinsic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Launcher.Service.Game;

internal sealed partial class LaunchOptions
{
    private static readonly Regex ResolutionRegex = new(@"(\d+)x(\d+)", RegexOptions.Compiled);

    public const int MinResolution = 320;
    public const int MaxWidth = 7680;
    public const int MaxHeight = 4320;

    [field: MaybeNull]
    public IObservableProperty<bool> IsFullScreen { get => field ??= CreateProperty(SettingKeys.LaunchIsFullScreen, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsBorderless { get => field ??= CreateProperty(SettingKeys.LaunchIsBorderless, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsWindowed { get => field ??= CreateProperty(SettingKeys.LaunchIsWindowed, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsExclusive { get => field ??= CreateProperty(SettingKeys.LaunchIsExclusive, false); }

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

    [field: MaybeNull]
    public IObservableProperty<int> ResolutionPresetIndex { get => field ??= CreateProperty(SettingKeys.LaunchResolutionPresetIndex, -1); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsScreenWidthEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsScreenWidthEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> ScreenWidth { get => field ??= CreateProperty(SettingKeys.LaunchScreenWidth, DisplayArea.Primary.OuterBounds.Width); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsScreenHeightEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsScreenHeightEnabled, true); }

    [field: MaybeNull]
    public IObservableProperty<int> ScreenHeight { get => field ??= CreateProperty(SettingKeys.LaunchScreenHeight, DisplayArea.Primary.OuterBounds.Height); }

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

    public void ClampResolution()
    {
        ScreenWidth.Value = Math.Clamp(ScreenWidth.Value, MinResolution, MaxWidth);
        ScreenHeight.Value = Math.Clamp(ScreenHeight.Value, MinResolution, MaxHeight);
    }

    private static ImmutableArray<NameValue<int>>? _commonResolutions;
    public static ImmutableArray<NameValue<int>> CommonResolutions => _commonResolutions ??= InitializeCommonResolutions();

    public static int CustomPresetIndex => CommonResolutions.Length;

    private static ImmutableArray<NameValue<int>> InitializeCommonResolutions()
    {
        return
        [
            new("1280x720 (720p HD)", 1280),
            new("1366x768 (WXGA)", 1366),
            new("1600x900 (HD+)", 1600),
            new("1920x1080 (1080p FHD)", 1920),
            new("2560x1440 (2K QHD)", 2560),
            new("3840x2160 (4K UHD)", 3840),
            new("1280x800 (WXGA)", 1280),
            new("1440x900 (WXGA+)", 1440),
            new("1680x1050 (WSXGA+)", 1680),
            new("1920x1200 (WUXGA)", 1920),
            new("2560x1600 (WQXGA)", 2560),
            new("1024x768 (XGA)", 1024),
            new("1280x960 (SXGA-)", 1280),
            new("1400x1050 (SXGA+)", 1400),
            new("1600x1200 (UXGA)", 1600),
            new("1920x1440 (SXGA)", 1920),
            new("2560x1080 (UWFHD 21:9)", 2560),
            new("3440x1440 (UWQHD 21:9)", 3440),
            new("5120x1440 (DUHD 32:9)", 5120),
        ];
    }

    public void ApplyResolutionPreset()
    {
        int index = ResolutionPresetIndex.Value;

        if (index == CustomPresetIndex)
        {
            IsScreenWidthEnabled.Value = true;
            IsScreenHeightEnabled.Value = true;
            return;
        }

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

    internal static int FindMatchingResolutionIndex(int width, int height)
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

        return CustomPresetIndex;
    }

    [field: MaybeNull]
    public IObservableProperty<bool> IsMonitorEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsMonitorEnabled, true); }

    public ImmutableArray<NameValue<int>> Monitors { get; } = InitializeMonitors();

    [field: MaybeNull]
    public IObservableProperty<NameValue<int>?> Monitor { get => field ??= CreatePropertyForSelectedOneBasedIndex(SettingKeys.LaunchMonitor, Monitors); }

    private static ImmutableArray<NameValue<int>> InitializeMonitors()
    {
        ImmutableArray<NameValue<int>>.Builder monitors = ImmutableArray.CreateBuilder<NameValue<int>>();
        try
        {
            IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
            for (int i = 0; i < displayAreas.Count; i++)
            {
                DisplayArea displayArea = displayAreas[i];
                int index = i + 1;
                monitors.Add(new($"{displayArea.DisplayId.Value:X8}:{index}", index));
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            monitors.Clear();
        }

        return monitors.ToImmutable();
    }

    [field: MaybeNull]
    public IObservableProperty<bool> IsPlatformTypeEnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsPlatformTypeEnabled, false); }

    public ImmutableArray<NameValue<PlatformType>> PlatformTypes { get; } = ImmutableCollectionsNameValue.FromEnum<PlatformType>();

    [field: MaybeNull]
    public IObservableProperty<PlatformType> PlatformType { get => field ??= CreateProperty(SettingKeys.LaunchPlatformType, Model.Intrinsic.PlatformType.PC); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsWindowsHDREnabled { get => field ??= CreateProperty(SettingKeys.LaunchIsWindowsHDREnabled, false); }
}
