//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Core.Property;
using Launcher.Core.Setting;
using Launcher.Service.Abstraction;
using Launcher.Service.Game.FileSystem;
using Launcher.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Launcher.Service.Game;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchOptions : DbStoreOptions, IRestrictedGamePathAccess
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial LaunchOptions(IServiceProvider serviceProvider);

    [field: MaybeNull]
    public static IObservableProperty<bool> IsGameRunning { get => field ??= GameLifeCycle.IsGameRunningProperty; }

    [field: MaybeNull]
    public static IReadOnlyObservableProperty<bool> CanKillGameProcess { get => field ??= Property.Observe(IsGameRunning, value => LauncherRuntime.IsProcessElevated && value); }

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
}
