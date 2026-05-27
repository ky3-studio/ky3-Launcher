//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using kyxsan.Core;
using kyxsan.Core.Property;
using kyxsan.Core.Setting;
using kyxsan.Model;
using kyxsan.Service.Abstraction;
using kyxsan.Service.BackgroundImage;
using kyxsan.UI.Xaml.Media.Backdrop;
using BackdropTypeEnum = kyxsan.UI.Xaml.Media.Backdrop.BackdropType;
using kyxsan.Web.Bridge;
using kyxsan.Web.Hoyolab;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

namespace kyxsan.Service;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial AppOptions(IServiceProvider serviceProvider);

    public static bool NotifyIconCreated { get => XamlApplicationLifetime.NotifyIconCreated; }

    public Lazy<ImmutableArray<NameValue<ElementTheme>>> LazyElementThemes { get; } = new(static () =>
    [
        new(SH.CoreWindowThemeLight, Microsoft.UI.Xaml.ElementTheme.Light),
        new(SH.CoreWindowThemeDark, Microsoft.UI.Xaml.ElementTheme.Dark),
        new(SH.CoreWindowThemeSystem, Microsoft.UI.Xaml.ElementTheme.Default),
    ]);

    public Lazy<ImmutableArray<NameValue<Region>>> LazyRegions { get; } = new(static () =>
    {
        Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
        return KnownRegions.Value;
    });

    public ImmutableArray<NameValue<BackdropType>> BackdropTypes { get; } = CreateBackdropTypes();

    private static ImmutableArray<NameValue<BackdropType>> CreateBackdropTypes()
    {
        return
        [
            new(SH.ServiceAppOptionsBackdropAcrylic, BackdropTypeEnum.Acrylic),
            new(SH.ServiceAppOptionsBackdropAcrylicThin, BackdropTypeEnum.AcrylicThin),
            new(SH.ServiceAppOptionsBackdropMica, BackdropTypeEnum.Mica),
            new(SH.ServiceAppOptionsBackdropMicaAlt, BackdropTypeEnum.MicaAlt),
        ];
    }

    public ImmutableArray<NameValue<BackgroundImageType>> BackgroundImageTypes { get; } = ImmutableCollectionsNameValue.FromEnum<BackgroundImageType>(type => type.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture) ?? string.Empty);

    public ImmutableArray<NameValue<int>> BackgroundSwitchIntervals { get; } =
    [
        new("5 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 5),
        new("8 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 8),
        new("10 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 10),
        new("15 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 15),
        new("20 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 20),
        new("30 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 30),
        new("60 " + SH.ViewPageSettingBackgroundSwitchIntervalUnit, 60),
    ];

    public ImmutableArray<NameValue<LastWindowCloseBehavior>> LastWindowCloseBehaviors { get; } = ImmutableCollectionsNameValue.FromEnum<LastWindowCloseBehavior>(static @enum => @enum.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture) ?? string.Empty);

    [field: MaybeNull]
    public IObservableProperty<BackdropType> BackdropType { get => field ??= CreateProperty(SettingKeys.SystemBackdropType, UI.Xaml.Media.Backdrop.BackdropType.AcrylicThin); }

    [field: MaybeNull]
    public IObservableProperty<ElementTheme> ElementTheme { get => field ??= CreateProperty(SettingKeys.ElementTheme, Microsoft.UI.Xaml.ElementTheme.Default); }

    [field: MaybeNull]
    public IObservableProperty<BackgroundImageType> BackgroundImageType { get => field ??= CreateProperty(SettingKeys.BackgroundImageType, BackgroundImage.BackgroundImageType.kyxsanOfficialLauncher); }

    [field: MaybeNull]
    public IObservableProperty<string> BackgroundImageCustomPath { get => field ??= CreateProperty(SettingKeys.BackgroundImageCustomPath, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> HomePageCardVisible { get => field ??= CreateProperty(SettingKeys.HomePageCardVisible, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> HomePageIndicatorVisible { get => field ??= CreateProperty(SettingKeys.HomePageIndicatorVisible, true); }

    [field: MaybeNull]
    public IObservableProperty<int> BackgroundSwitchInterval { get => field ??= CreateProperty(SettingKeys.BackgroundSwitchInterval, 8); }

    [field: MaybeNull]
    public IObservableProperty<bool> BackgroundShowDynamic { get => field ??= CreateProperty(SettingKeys.BackgroundShowDynamic, true); }

    [field: MaybeNull]
    public IObservableProperty<bool> BackgroundShowStatic { get => field ??= CreateProperty(SettingKeys.BackgroundShowStatic, true); }

    [field: MaybeNull]
    public IObservableProperty<Region> Region { get => field ??= CreatePropertyForStructUsingCustom(SettingKeys.AnnouncementRegion, Web.Hoyolab.Region.CNGF01, Web.Hoyolab.Region.FromRegionString, Web.Hoyolab.Region.ToRegionString); }

    [field: MaybeNull]
    public IObservableProperty<string> GeetestCustomCompositeUrl { get => field ??= CreateProperty(SettingKeys.GeetestCustomCompositeUrl, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<BridgeShareSaveType> BridgeShareSaveType { get => field ??= CreateProperty(SettingKeys.BridgeShareSaveType, Web.Bridge.BridgeShareSaveType.CopyToClipboard); }

    [field: MaybeNull]
    public IObservableProperty<LastWindowCloseBehavior> LastWindowCloseBehavior { get => field ??= CreateProperty(SettingKeys.LastWindowCloseBehavior, Service.LastWindowCloseBehavior.ExitApplication); }

    [field: MaybeNull]
    public IObservableProperty<bool> RememberWindowSize { get => field ??= CreateProperty(SettingKeys.RememberWindowSize, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsStartupEnabled { get => field ??= CreateProperty(SettingKeys.StartupEnabled, false).WithValueChangedCallback(OnStartupEnabledChanged, this); }

    [field: MaybeNull]
    public IObservableProperty<bool> RunElevated { get => field ??= CreateProperty(SettingKeys.RunElevated, false).WithValueChangedCallback(OnRunElevatedChanged, this); }

    private static void OnStartupEnabledChanged(bool value, AppOptions options)
    {
        try
        {
            IServiceProvider sp = Ioc.Default;
            AutoStartService autoStart = sp.GetRequiredService<AutoStartService>();
            autoStart.SetStartup(value, options.RunElevated.Value);
        }
        catch
        {
        }
    }

    private static void OnRunElevatedChanged(bool value, AppOptions options)
    {
        try
        {
            LocalSetting.Set(SettingKeys.RunElevated, value);

            if (kyxsanRuntime.IsProcessElevated && options.IsStartupEnabled.Value)
            {
                IServiceProvider sp = Ioc.Default;
                AutoStartService autoStart = sp.GetRequiredService<AutoStartService>();
                autoStart.SetStartup(true, value);
            }
        }
        catch
        {
        }
    }
}