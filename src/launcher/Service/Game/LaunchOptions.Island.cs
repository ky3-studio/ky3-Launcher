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
using System.Collections.Immutable;

namespace Launcher.Service.Game;

internal sealed partial class LaunchOptions
{
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

    [field: MaybeNull]
    public IObservableProperty<bool> EnableNoGrass { get => field ??= CreateProperty(SettingKeys.LaunchEnableNoGrass, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnableGui { get => field ??= CreateProperty(SettingKeys.LaunchEnableGui, true); }

    [field: MaybeNull]
    public IObservableProperty<int> GuiKey { get => field ??= CreateProperty(SettingKeys.LaunchGuiKey, 0xA1); }

    [field: MaybeNull]
    public IObservableProperty<int> GuiModifier { get => field ??= CreateProperty(SettingKeys.LaunchGuiModifier, 0); }

    [field: MaybeNull]
    public IObservableProperty<bool> EnableFreeCam { get => field ??= CreateProperty(SettingKeys.LaunchEnableFreeCam, false); }

    [field: MaybeNull]
    public IObservableProperty<int> FreeCamKey { get => field ??= CreateProperty(SettingKeys.LaunchFreeCamKey, 0x75); }

    [field: MaybeNull]
    public IObservableProperty<int> FreeCamModifier { get => field ??= CreateProperty(SettingKeys.LaunchFreeCamModifier, 0); }

    [field: MaybeNull]
    public IObservableProperty<float> FreeCamMoveSpeed { get => field ??= CreateProperty(SettingKeys.LaunchFreeCamMoveSpeed, 0.35f); }

    [field: MaybeNull]
    public IObservableProperty<float> FreeCamSprintMult { get => field ??= CreateProperty(SettingKeys.LaunchFreeCamSprintMult, 3.0f); }

    [field: MaybeNull]
    public IObservableProperty<float> FreeCamMouseSensitivity { get => field ??= CreateProperty(SettingKeys.LaunchFreeCamMouseSensitivity, 0.12f); }

    [field: MaybeNull]
    public IObservableProperty<float> FreeCamPitchLimit { get => field ??= CreateProperty(SettingKeys.LaunchFreeCamPitchLimit, 89f); }

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
    public IObservableProperty<ImmutableDictionary<string, bool>> CustomDllConfigs { get => field ??= CreatePropertyForClassUsingCustom(SettingKeys.LaunchCustomDllConfigs, ImmutableDictionary<string, bool>.Empty, s => JsonSerializer.Deserialize<ImmutableDictionary<string, bool>>(s) ?? ImmutableDictionary<string, bool>.Empty, d => JsonSerializer.Serialize(d)); }
}
