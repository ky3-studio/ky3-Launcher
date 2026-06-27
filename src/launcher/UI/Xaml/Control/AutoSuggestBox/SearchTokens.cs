//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Model.Entity.Primitive.Converter;
using Launcher.Model.Intrinsic;
using Launcher.Model.Intrinsic.Frozen;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Converter;
using Launcher.Model.Metadata.Food;
using Launcher.Model.Metadata.Quest;
using Launcher.Model.Metadata.Weapon;
using Launcher.Web.Endpoint.Launcher;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Launcher.UI.Xaml.Control.AutoSuggestBox;

internal static class SearchTokens
{
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> ElementTokens = [.. IntrinsicFrozen.ElementNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ElementName, nv.Name, nv.Value, iconUri: ElementNameIconConverter.ElementNameToUri(nv.Name))))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> ItemQuality45Tokens = [.. IntrinsicFrozen.ItemQualityNameValues.Where(static nv => nv.Value >= QualityType.QUALITY_PURPLE).Select(nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ItemQuality, nv.Name, (int)nv.Value, quality: QualityColorConverter.QualityToColor(nv.Value))))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> ItemQualityAllTokens = [.. IntrinsicFrozen.ItemQualityNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.ItemQuality, nv.Name, (int)nv.Value, quality: QualityColorConverter.QualityToColor(nv.Value))))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> WeaponTypeTokens = [.. IntrinsicFrozen.WeaponTypeNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.WeaponType, nv.Name, (int)nv.Value, iconUri: WeaponTypeIconConverter.WeaponTypeToIconUri(nv.Value))))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> AssociationTypeTokens = [.. IntrinsicFrozen.AssociationTypeNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.AssociationType, nv.Name, (int)nv.Value, iconUri: AssociationTypeIconConverter.AssociationTypeToIconUri(nv.Value))))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> BodyTypeTokens = [.. IntrinsicFrozen.BodyTypeNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.BodyType, nv.Name, (int)nv.Value)))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> FightPropertyTokens = [.. IntrinsicFrozen.FightPropertyNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.FightProperty, nv.Name, (int)nv.Value)))];
    private static readonly ImmutableArray<KeyValuePair<string, SearchToken>> CultivateTypeTokens = [.. IntrinsicFrozen.CultivateTypeNameValues.Select(static nv => KeyValuePair.Create(nv.Name, new SearchToken(SearchTokenKind.CultivateType, nv.Name, (int)nv.Value, packageIconUri: CultivateTypeIconConverter.CultivateTypeToIconUri(nv.Value))))];

    private static readonly LazySlim<FrozenDictionary<string, SearchToken>> AvatarPropertyTokens = new(() => WinRTAdaptive.ToFrozenDictionary(
    [.. ElementTokens, .. ItemQuality45Tokens, .. WeaponTypeTokens]));

    private static readonly LazySlim<FrozenDictionary<string, SearchToken>> CultivationTokens = new(() => WinRTAdaptive.ToFrozenDictionary(
    [.. CultivateTypeTokens, .. ElementTokens, .. ItemQualityAllTokens, .. WeaponTypeTokens]));

    public static FrozenDictionary<string, SearchToken> GetForAvatarProperty()
    {
        return AvatarPropertyTokens.Value;
    }

    public static FrozenDictionary<string, SearchToken> GetForCultivation()
    {
        return CultivationTokens.Value;
    }

    public static FrozenDictionary<string, SearchToken> GetForWikiAvatar(ImmutableArray<Avatar> array)
    {
        return WinRTAdaptive.ToFrozenDictionary(
        [
            .. array.Select((avatar, index) => KeyValuePair.Create(avatar.Name, new SearchToken(SearchTokenKind.Avatar, avatar.Name, index, sideIconUri: AvatarSideIconConverter.IconNameToUri(avatar.SideIcon)))),
            .. AssociationTypeTokens,
            .. BodyTypeTokens,
            .. ElementTokens,
            .. ItemQuality45Tokens,
            .. WeaponTypeTokens,
        ]);
    }

    public static FrozenDictionary<string, SearchToken> GetForWikiWeapon(ImmutableArray<Weapon> array)
    {
        return WinRTAdaptive.ToFrozenDictionary(
        [
            .. array.Select((weapon, index) => KeyValuePair.Create(weapon.Name, new SearchToken(SearchTokenKind.Weapon, weapon.Name, index, sideIconUri: EquipIconConverter.IconNameToUri(weapon.Icon)))),
            .. FightPropertyTokens,
            .. ItemQualityAllTokens,
            .. WeaponTypeTokens,
        ]);
    }

    public static FrozenDictionary<string, SearchToken> GetForArchonQuest(ImmutableArray<ArchonQuest> array)
    {
        FrozenSet<string> validRegions = FrozenSet.ToFrozenSet(AssociationTypeTokens.Select(t => t.Value.Value));

        ImmutableArray<KeyValuePair<string, SearchToken>> regionTokens = [.. array
            .Select(q => q.Region)
            .Distinct()
            .Where(r => validRegions.Contains(r))
            .Select((region, index) =>
            {
                Uri? iconUri = AssociationTypeTokens
                    .Where(t => t.Value.Value == region)
                    .Select(t => t.Value.IconUri)
                    .FirstOrDefault();
                return KeyValuePair.Create(region, new SearchToken(
                    SearchTokenKind.Region, region, index, iconUri: iconUri));
            })];

        return WinRTAdaptive.ToFrozenDictionary([.. regionTokens]);
    }

    public static FrozenDictionary<string, SearchToken> GetForWikiFood(ImmutableArray<Food> array)
    {
        ImmutableArray<KeyValuePair<string, SearchToken>> rankTokens = [.. Enumerable.Range(1, 5).Select(r =>
        {
            string label = string.Format(SH.ViewWikiFoodRankStar, r);
            return KeyValuePair.Create(label, new SearchToken(SearchTokenKind.ItemQuality, label, r));
        })];

        ImmutableArray<KeyValuePair<string, SearchToken>> effectTokens = [.. array
            .Select(f => f.EffectIcon)
            .Where(e => !string.IsNullOrEmpty(e))
            .Distinct()
            .Select((effectIcon, index) =>
            {
                string label = effectIcon switch
                {
                    "UI_Buff_Item_Recovery_HpAdd" => SH.ViewWikiFoodEffectRecoveryHp,
                    "UI_Buff_Item_Recovery_HpAddAll" => SH.ViewWikiFoodEffectRecoveryHpAll,
                    "UI_Buff_Item_Recovery_Revive" => SH.ViewWikiFoodEffectRevive,
                    "UI_Buff_Item_Atk_Add" => SH.ViewWikiFoodEffectAtkAdd,
                    "UI_Buff_Item_Atk_CritRate" => SH.ViewWikiFoodEffectCritRate,
                    "UI_Buff_Item_Def_Add" => SH.ViewWikiFoodEffectDefAdd,
                    "UI_Buff_Item_Other_SPAdd" => SH.ViewWikiFoodEffectSPAdd,
                    "UI_Buff_Item_Other_SPReduceConsume" => SH.ViewWikiFoodEffectSPReduce,
                    "UI_Buff_Item_Climate_Heat" => SH.ViewWikiFoodEffectClimate,
                    "UI_Buff_Item_Adventure" => SH.ViewWikiFoodEffectAdventure,
                    "UI_Buff_Item_SpecialEffect" => SH.ViewWikiFoodEffectSpecial,
                    _ => effectIcon,
                };
                Uri iconUri = StaticResourcesEndpoints.StaticRaw("BuffIcon", $"{effectIcon}.png").ToUri();
                return KeyValuePair.Create(label, new SearchToken(SearchTokenKind.FoodEffect, label, index, iconUri: iconUri));
            })];

        ImmutableArray<KeyValuePair<string, SearchToken>> nameTokens = [.. array.Select((food, index) =>
            KeyValuePair.Create(food.Name, new SearchToken(SearchTokenKind.Food, food.Name, index, iconUri: ItemIconConverter.IconNameToUri(food.Icon))))];

        string categoryLabel = SH.ViewWikiFoodCategorySpecial;
        ImmutableArray<KeyValuePair<string, SearchToken>> categoryTokens = [KeyValuePair.Create(categoryLabel, new SearchToken(SearchTokenKind.FoodCategory, categoryLabel, 0))];

        return WinRTAdaptive.ToFrozenDictionary([.. nameTokens, .. rankTokens, .. effectTokens, .. categoryTokens]);
    }
}