//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata.Converter;
using Launcher.Model.Primitive;
using Launcher.UI.Xaml.Data;
using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;
using MetaAvatar = Launcher.Model.Metadata.Avatar.Avatar;
using MetaWeapon = Launcher.Model.Metadata.Weapon.Weapon;

namespace Launcher.ViewModel.AvatarProperty;

internal sealed partial class CharacterView : IPropertyValuesProvider
{
    public CharacterView(Character apiCharacter, MetaAvatar metaAvatar, MetaWeapon? metaWeapon)
    {
        Id = apiCharacter.Id;
        Name = apiCharacter.Name;
        Level = apiCharacter.Level;
        LevelNumber = (uint)apiCharacter.Level;
        LevelFormatted = LevelFormat.Format(apiCharacter.Level);
        Element = apiCharacter.Element;
        ElementLocalizedName = metaAvatar.FetterInfo?.VisionBefore ?? string.Empty;
        Rarity = apiCharacter.Rarity;
        Quality = apiCharacter.Rarity;
        ActivedConstellationNum = apiCharacter.ActivedConstellationNum;
        ActivatedConstellationCount = apiCharacter.ActivedConstellationNum;
        FetterLevel = apiCharacter.Fetter;
        WeaponType = apiCharacter.Weapon.Type;
        WeaponId = apiCharacter.Weapon.Id;
        WeaponLevelNumber = (uint)apiCharacter.Weapon.Level;
        Icon = AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = AvatarSideIconConverter.IconNameToUri(metaAvatar.SideIcon);

        NameCard = metaAvatar.NameCard is { PicturePrefix.Length: > 0 } nc
            ? AvatarNameCardPicConverter.IconNameToUri(nc.PicturePrefix)
            : Icon;

        if (metaWeapon is not null)
        {
            WeaponIcon = EquipIconConverter.IconNameToUri(metaWeapon.Icon);
        }

        WeaponRarity = apiCharacter.Weapon.Rarity;
        WeaponAffixLevelNumber = apiCharacter.Weapon.AffixLevel;
        WeaponLevel = LevelFormat.Format(apiCharacter.Weapon.Level);
    }

    public AvatarId Id { get; }

    public string Name { get; }

    public Level Level { get; }

    public uint LevelNumber { get; }

    public string LevelFormatted { get; }

    public ElementName Element { get; }

    public string ElementLocalizedName { get; }

    public QualityType Rarity { get; }

    public QualityType Quality { get; }

    public int ActivedConstellationNum { get; }

    public int ActivatedConstellationCount { get; }

    public FetterLevel FetterLevel { get; }

    public WeaponType WeaponType { get; }

    public Uri Icon { get; }

    public Uri SideIcon { get; }

    public Uri NameCard { get; }

    public WeaponId WeaponId { get; }

    public uint WeaponLevelNumber { get; }

    public Uri? WeaponIcon { get; }

    public QualityType WeaponRarity { get; }

    public uint WeaponAffixLevelNumber { get; }

    public string WeaponLevel { get; }

    public ImmutableArray<SkillView> Skills { get; private set; } = [];

    public void SetSkills(ImmutableArray<Skill> apiSkills, MetaAvatar metaAvatar)
    {
        if (metaAvatar.SkillDepot is null)
        {
            return;
        }

        ImmutableArray<Model.Metadata.Avatar.ProudSkill> proudSkills = metaAvatar.SkillDepot.CompositeSkillsNoInherents;
        ImmutableArray<SkillView>.Builder builder = ImmutableArray.CreateBuilder<SkillView>(proudSkills.Length);

        foreach (Model.Metadata.Avatar.ProudSkill proudSkill in proudSkills)
        {
            SkillLevel level = (SkillLevel)1U;
            SkillType skillType = default;

            foreach (Skill apiSkill in apiSkills)
            {
                if (apiSkill.SkillId == proudSkill.Id)
                {
                    level = apiSkill.Level;
                    skillType = apiSkill.SkillType;
                    break;
                }
            }

            builder.Add(new SkillView
            {
                Name = proudSkill.Name,
                Icon = SkillIconConverter.IconNameToUri(proudSkill.Icon),
                Level = level,
                LevelFormatted = LevelFormat.Format((uint)level),
                SkillType = skillType,
                Description = string.Empty,
                Info = new(string.Empty, []),
            });
        }

        Skills = builder.ToImmutable();
    }
}
