// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Metadata.Weapon;

namespace kyxsan.UI.Xaml.View.Dialog;

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
