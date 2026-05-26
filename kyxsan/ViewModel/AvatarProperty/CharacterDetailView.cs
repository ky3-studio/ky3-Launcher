//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Primitive;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;
using MetaAvatar = kyxsan.Model.Metadata.Avatar.Avatar;
using MetaWeapon = kyxsan.Model.Metadata.Weapon.Weapon;

namespace kyxsan.ViewModel.AvatarProperty;

internal sealed class CharacterDetailView
{
    public CharacterDetailView(DetailedCharacter detail, MetaAvatar metaAvatar, MetaWeapon metaWeapon)
    {
        Name = detail.Base.Name;
        Level = detail.Base.Level;
        LevelFormatted = LevelFormat.Format(detail.Base.Level);
        Element = detail.Base.Element;
        Rarity = detail.Base.Rarity;
        Fetter = detail.Base.Fetter;
        ActivedConstellationNum = detail.Base.ActivedConstellationNum;

        SplashImage = GachaAvatarImgConverter.IconNameToUri(metaAvatar.Icon);
        Icon = AvatarIconConverter.IconNameToUri(metaAvatar.Icon);

        Constellations = detail.Constellations;
        Skills = BuildSkillViews(detail.Skills, metaAvatar);

        WeaponName = metaWeapon.Name;
        WeaponIcon = EquipIconConverter.IconNameToUri(metaWeapon.Icon);
        WeaponRarity = detail.Weapon.Rarity;
        WeaponLevel = detail.Weapon.Level;
        WeaponLevelFormatted = LevelFormat.Format(detail.Weapon.Level);
        WeaponAffixLevel = detail.Weapon.AffixLevel;

        Properties = detail.SelectedProperties;
        Relics = detail.Relics;
    }

    public string Name { get; }

    public Level Level { get; }

    public string LevelFormatted { get; }

    public ElementName Element { get; }

    public QualityType Rarity { get; }

    public FetterLevel Fetter { get; }

    public int ActivedConstellationNum { get; }

    public Uri SplashImage { get; }

    public Uri Icon { get; }

    public ImmutableArray<Constellation> Constellations { get; }

    public ImmutableArray<SkillView> Skills { get; }

    public string WeaponName { get; }

    public Uri WeaponIcon { get; }

    public QualityType WeaponRarity { get; }

    public Level WeaponLevel { get; }

    public string WeaponLevelFormatted { get; }

    public uint WeaponAffixLevel { get; }

    public ImmutableArray<BaseProperty> Properties { get; }

    public ImmutableArray<Reliquary> Relics { get; }

    private static ImmutableArray<SkillView> BuildSkillViews(ImmutableArray<Skill> apiSkills, MetaAvatar metaAvatar)
    {
        ImmutableArray<SkillView>.Builder builder = ImmutableArray.CreateBuilder<SkillView>(apiSkills.Length);

        foreach (Skill apiSkill in apiSkills)
        {
            string icon = string.Empty;
            string name = string.Empty;

            foreach (Model.Metadata.Avatar.ProudSkill metaSkill in metaAvatar.SkillDepot.CompositeSkills)
            {
                if (metaSkill.Id == apiSkill.SkillId)
                {
                    icon = metaSkill.Icon;
                    name = metaSkill.Name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(icon))
            {
                foreach (Model.Metadata.Avatar.Skill talent in metaAvatar.SkillDepot.Talents)
                {
                    if (talent.Id == apiSkill.SkillId)
                    {
                        icon = talent.Icon;
                        name = talent.Name;
                        break;
                    }
                }
            }

            builder.Add(new SkillView
            {
                Name = name,
                Icon = SkillIconConverter.IconNameToUri(icon),
                Level = apiSkill.Level,
                SkillType = apiSkill.SkillType,
            });
        }

        return builder.ToImmutable();
    }
}

internal sealed class SkillView
{
    public required string Name { get; init; }

    public required Uri Icon { get; init; }

    public required SkillLevel Level { get; init; }

    public required SkillType SkillType { get; init; }
}
