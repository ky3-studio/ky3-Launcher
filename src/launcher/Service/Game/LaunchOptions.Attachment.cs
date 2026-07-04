//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Property;
using Launcher.Core.Setting;

namespace Launcher.Service.Game;

internal sealed partial class LaunchOptions
{
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
    public IObservableProperty<string> AdvancedStartProgramPath { get => field ??= CreateProperty(SettingKeys.LaunchAdvancedStartProgramPath, string.Empty); }

    [field: MaybeNull]
    public IObservableProperty<bool> AdvancedStartDelayedOnAdvancedStart { get => field ??= CreateProperty(SettingKeys.LaunchAdvancedStartDelayedOnAdvancedStart, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> AdvancedStartDelayedOnGameLaunch { get => field ??= CreateProperty(SettingKeys.LaunchAdvancedStartDelayedOnGameLaunch, false); }
}
