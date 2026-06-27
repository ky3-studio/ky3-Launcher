// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata.Converter;
using Launcher.Model.Metadata.Weapon;

namespace Launcher.UI.Xaml.View.Dialog;

internal sealed partial class CultivateWeaponLevelItem : ObservableObject
{
    public required Weapon Weapon { get; init; }

    public string WeaponName => Weapon.Name;

    public Uri WeaponIcon => EquipIconConverter.IconNameToUri(Weapon.Icon);

    public QualityType WeaponQuality => Weapon.Quality;

    [ObservableProperty]
    public partial double LevelFrom { get; set; } = 1;

    [ObservableProperty]
    public partial double LevelTo { get; set; } = 90;

    public static CultivateWeaponLevelItem Create(Weapon weapon)
    {
        uint maxLevel = Weapon.GetMaxLevelByQuality(weapon.Quality);
        return new()
        {
            Weapon = weapon,
            LevelTo = maxLevel,
        };
    }
}
