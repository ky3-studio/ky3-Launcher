// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.ViewModel.AvatarProperty;
using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.View.Dialog;

internal sealed partial class CultivateAvatarLevelItem : ObservableObject
{
    public required Avatar Avatar { get; init; }

    public CharacterView? CharacterView { get; init; }

    public Weapon? Weapon { get; init; }

    public string AvatarName => Avatar.Name;

    public Uri AvatarIcon => AvatarIconConverter.IconNameToUri(Avatar.Icon);

    public QualityType AvatarQuality => Avatar.Quality;

    public Uri? SkillAIcon { get; init; }

    public string? SkillAName { get; init; }

    public Uri? SkillEIcon { get; init; }

    public string? SkillEName { get; init; }

    public Uri? SkillQIcon { get; init; }

    public string? SkillQName { get; init; }

    public bool HasWeapon => Weapon is not null;

    public string? WeaponName { get; set; }

    public Uri? WeaponIcon { get; set; }

    public QualityType WeaponQuality { get; set; }

    [ObservableProperty]
    public partial double AvatarLevelFrom { get; set; } = 1;

    [ObservableProperty]
    public partial double AvatarLevelTo { get; set; } = 90;

    [ObservableProperty]
    public partial double SkillALevelFrom { get; set; } = 1;

    [ObservableProperty]
    public partial double SkillALevelTo { get; set; } = 10;

    [ObservableProperty]
    public partial double SkillELevelFrom { get; set; } = 1;

    [ObservableProperty]
    public partial double SkillELevelTo { get; set; } = 10;

    [ObservableProperty]
    public partial double SkillQLevelFrom { get; set; } = 1;

    [ObservableProperty]
    public partial double SkillQLevelTo { get; set; } = 10;

    [ObservableProperty]
    public partial double WeaponLevelFrom { get; set; } = 1;

    [ObservableProperty]
    public partial double WeaponLevelTo { get; set; } = 90;

    [ObservableProperty]
    public partial bool WeaponAscended { get; set; }

    [ObservableProperty]
    public partial bool WeaponPromotionAvailable { get; set; }

    partial void OnWeaponLevelFromChanged(double value)
    {
        uint level = (uint)value;
        WeaponPromotionAvailable = level is 20 or 40 or 50 or 60 or 70 or 80;
        if (!WeaponPromotionAvailable)
        {
            WeaponAscended = false;
        }
    }

    public CultivateLevelInput ToLevelInput()
    {
        return new()
        {
            AvatarLevelFrom = (uint)AvatarLevelFrom,
            AvatarLevelTo = (uint)AvatarLevelTo,
            SkillALevelFrom = (uint)SkillALevelFrom,
            SkillALevelTo = (uint)SkillALevelTo,
            SkillELevelFrom = (uint)SkillELevelFrom,
            SkillELevelTo = (uint)SkillELevelTo,
            SkillQLevelFrom = (uint)SkillQLevelFrom,
            SkillQLevelTo = (uint)SkillQLevelTo,
            WeaponLevelFrom = HasWeapon ? (uint)WeaponLevelFrom : 0,
            WeaponLevelTo = HasWeapon ? (uint)WeaponLevelTo : 0,
            WeaponAscended = HasWeapon && WeaponAscended,
        };
    }

    public static CultivateAvatarLevelItem Create(Avatar avatar, CharacterView? characterView, Weapon? weapon)
    {
        ImmutableArray<ProudSkill> skills = avatar.SkillDepot.CompositeSkillsNoInherents;

        CultivateAvatarLevelItem item = new()
        {
            Avatar = avatar,
            CharacterView = characterView,
            Weapon = weapon,
            SkillAIcon = skills.Length >= 1 ? SkillIconConverter.IconNameToUri(skills[0].Icon) : null,
            SkillAName = skills.Length >= 1 ? skills[0].Name : null,
            SkillEIcon = skills.Length >= 2 ? SkillIconConverter.IconNameToUri(skills[1].Icon) : null,
            SkillEName = skills.Length >= 2 ? skills[1].Name : null,
            SkillQIcon = skills.Length >= 3 ? SkillIconConverter.IconNameToUri(skills[2].Icon) : null,
            SkillQName = skills.Length >= 3 ? skills[2].Name : null,
        };

        if (characterView is not null)
        {
            item.AvatarLevelFrom = characterView.LevelNumber;
            ImmutableArray<SkillView> charSkills = characterView.Skills;
            if (charSkills.Length >= 1)
            {
                item.SkillALevelFrom = (uint)charSkills[0].Level;
            }

            if (charSkills.Length >= 2)
            {
                item.SkillELevelFrom = (uint)charSkills[1].Level;
            }

            if (charSkills.Length >= 3)
            {
                item.SkillQLevelFrom = (uint)charSkills[2].Level;
            }
        }

        if (weapon is not null)
        {
            item.WeaponName = weapon.Name;
            item.WeaponIcon = EquipIconConverter.IconNameToUri(weapon.Icon);
            item.WeaponQuality = weapon.Quality;
            uint maxLevel = Weapon.GetMaxLevelByQuality(weapon.Quality);
            item.WeaponLevelTo = maxLevel;

            if (characterView is not null)
            {
                item.WeaponLevelFrom = characterView.WeaponLevelNumber;
            }
        }

        return item;
    }
}
