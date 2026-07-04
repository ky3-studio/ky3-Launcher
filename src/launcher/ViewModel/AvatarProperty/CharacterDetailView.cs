//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model;
using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata;
using Launcher.Model.Metadata.Converter;
using Launcher.Model.Metadata.Weapon;
using Launcher.Model.Primitive;
using Launcher.ViewModel.Wiki;
using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using MetaAvatar = Launcher.Model.Metadata.Avatar.Avatar;
using MetaWeapon = Launcher.Model.Metadata.Weapon.Weapon;

namespace Launcher.ViewModel.AvatarProperty;

internal sealed class CharacterDetailView
{
    public CharacterDetailView(DetailedCharacter detail, MetaAvatar metaAvatar, MetaWeapon metaWeapon, AvatarPropertyMetadataContext metadataContext)
    {
        Name = detail.Base.Name;
        Level = detail.Base.Level;
        LevelFormatted = LevelFormat.Format(detail.Base.Level);
        Element = detail.Base.Element;
        Rarity = detail.Base.Rarity;
        Fetter = detail.Base.Fetter;
        ActivedConstellationNum = detail.Base.ActivedConstellationNum;
        PromoteArray = BuildPromoteArrayFromLevel((uint)detail.Base.Level);

        SplashImage = GachaAvatarImgConverter.IconNameToUri(metaAvatar.Icon);
        Icon = AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        NameCard = metaAvatar.NameCard is { PicturePrefix.Length: > 0 } nc
            ? AvatarNameCardPicConverter.IconNameToUri(nc.PicturePrefix)
            : SplashImage;

        Constellations = detail.Constellations;
        Skills = BuildSkillViews(detail.Skills, metaAvatar);

        WeaponName = metaWeapon.Name;
        WeaponIcon = EquipIconConverter.IconNameToUri(metaWeapon.Icon);
        WeaponRarity = detail.Weapon.Rarity;
        WeaponLevel = detail.Weapon.Level;
        WeaponLevelFormatted = LevelFormat.Format(detail.Weapon.Level);
        WeaponAffixLevel = detail.Weapon.AffixLevel;
        WeaponAffixFormatted = SH.FormatModelBindingAvatarPropertyWeaponAffix(detail.Weapon.AffixLevel);
        WeaponPromoteArray = BuildPromoteArray(detail.Weapon.PromoteLevel);

        WeaponAffixDescription = string.Empty;
        if (metaWeapon.Affix is { Descriptions.Length: > 0 } affix)
        {
            int affixIndex = (int)detail.Weapon.AffixLevel - 1;
            if (affixIndex >= 0 && affixIndex < affix.Descriptions.Length)
            {
                WeaponAffixDescription = affix.Descriptions[affixIndex].Description;
            }
        }

        // Weapon main/sub properties
        BaseValueInfoMetadataContext weaponBaseValueContext = new()
        {
            GrowCurveMap = metadataContext.LevelDictionaryWeaponGrowCurveMap,
            PromoteMap = metadataContext.IdDictionaryWeaponLevelPromoteMap.GetValueOrDefault(metaWeapon.PromoteId),
        };

        ImmutableArray<NameValue<string>> weaponBaseValues = BaseValueInfoConverter.ToNameValues(
            metaWeapon.GrowCurves.ToPropertyCurveValues(),
            detail.Weapon.Level,
            detail.Weapon.PromoteLevel,
            weaponBaseValueContext);

        WeaponMainProperty = weaponBaseValues.Length > 0 ? weaponBaseValues[0] : null;
        WeaponSubProperty = weaponBaseValues.Length > 1 ? weaponBaseValues[1] : null;

        Properties = detail.SelectedProperties;
        Relics = detail.Relics;

        RecommendedSandProperties = BuildRecommendedPropertyStrings(detail.RecommendRelicProperty?.RecommendProperties?.SandMainPropertyList ?? []);
        RecommendedGobletProperties = BuildRecommendedPropertyStrings(detail.RecommendRelicProperty?.RecommendProperties?.GobletMainPropertyList ?? []);
        RecommendedCircletProperties = BuildRecommendedPropertyStrings(detail.RecommendRelicProperty?.RecommendProperties?.CircletMainPropertyList ?? []);
        RecommendedSubProperties = BuildRecommendedPropertyStrings(detail.RecommendRelicProperty?.RecommendProperties?.SubPropertyList ?? []);
    }

    public string Name { get; }

    public Level Level { get; }

    public string LevelFormatted { get; }

    public ElementName Element { get; }

    public QualityType Rarity { get; }

    public FetterLevel Fetter { get; }

    public int ActivedConstellationNum { get; }

    public ImmutableArray<bool> PromoteArray { get; }

    public Uri SplashImage { get; }

    public Uri NameCard { get; }

    public Uri Icon { get; }

    public ImmutableArray<Constellation> Constellations { get; }

    public ImmutableArray<SkillView> Skills { get; }

    public string WeaponName { get; }

    public Uri WeaponIcon { get; }

    public QualityType WeaponRarity { get; }

    public Level WeaponLevel { get; }

    public string WeaponLevelFormatted { get; }

    public uint WeaponAffixLevel { get; }

    public string WeaponAffixFormatted { get; }

    public string WeaponAffixDescription { get; }

    public NameValue<string>? WeaponMainProperty { get; }

    public NameValue<string>? WeaponSubProperty { get; }

    public ImmutableArray<bool> WeaponPromoteArray { get; }

    public ImmutableArray<BaseProperty> Properties { get; }

    public ImmutableArray<Reliquary> Relics { get; }

    public ImmutableArray<string?> RecommendedSandProperties { get; }

    public ImmutableArray<string?> RecommendedGobletProperties { get; }

    public ImmutableArray<string?> RecommendedCircletProperties { get; }

    public ImmutableArray<string?> RecommendedSubProperties { get; }

    private static ImmutableArray<bool> BuildPromoteArray(PromoteLevel promoteLevel)
    {
        bool[] promoteArray = new bool[6];
        promoteArray.AsSpan(0, (int)(uint)promoteLevel).Fill(true);
        return ImmutableCollectionsMarshal.AsImmutableArray(promoteArray);
    }

    private static ImmutableArray<bool> BuildPromoteArrayFromLevel(uint level)
    {
        int promote = level switch
        {
            > 80 => 6,
            > 70 => 5,
            > 60 => 4,
            > 50 => 3,
            > 40 => 2,
            > 20 => 1,
            _ => 0,
        };

        bool[] promoteArray = new bool[6];
        promoteArray.AsSpan(0, promote).Fill(true);
        return ImmutableCollectionsMarshal.AsImmutableArray(promoteArray);
    }

    private static ImmutableArray<SkillView> BuildSkillViews(ImmutableArray<Skill> apiSkills, MetaAvatar metaAvatar)
    {
        if (metaAvatar.SkillDepot is null)
        {
            return [];
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

            LevelParameters<string, ParameterDescription> info = DescriptionsParametersDescriptor.Convert(proudSkill.Proud, (uint)level);

            builder.Add(new SkillView
            {
                Name = proudSkill.Name,
                Icon = SkillIconConverter.IconNameToUri(proudSkill.Icon),
                Level = level,
                LevelFormatted = LevelFormat.Format((uint)level),
                SkillType = skillType,
                Description = proudSkill.Description,
                Info = info,
            });
        }

        return builder.ToImmutable();
    }

    private static ImmutableArray<string?> BuildRecommendedPropertyStrings(ImmutableArray<FightProperty> properties)
    {
        if (properties.IsDefaultOrEmpty)
        {
            return [];
        }

        return properties.SelectAsArray(static p => p.GetLocalizedDescription(SH.ResourceManager));
    }
}

internal sealed class SkillView
{
    public required string Name { get; init; }

    public required Uri Icon { get; init; }

    public required SkillLevel Level { get; init; }

    public required string LevelFormatted { get; init; }

    public required SkillType SkillType { get; init; }

    public required string Description { get; init; }

    public required LevelParameters<string, ParameterDescription> Info { get; init; }
}
