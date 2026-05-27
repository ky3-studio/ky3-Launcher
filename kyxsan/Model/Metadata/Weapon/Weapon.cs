//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Primitive;
using kyxsan.UI.Xaml.Data;
using kyxsan.ViewModel.Complex;
using kyxsan.ViewModel.GachaLog;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Weapon;

internal sealed partial class Weapon : IDefaultIdentity<WeaponId>,
    INameQualityAccess,
    IStatisticsItemConvertible,
    ISummaryItemConvertible,
    IItemConvertible,
    IPropertyValuesProvider
{
    public required WeaponId Id { get; init; }

    public required PromoteId PromoteId { get; init; }

    public required uint Sort { get; init; }

    public required WeaponType WeaponType { get; init; }

    public required QualityType RankLevel { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Icon { get; init; }

    public required string AwakenIcon { get; init; }

    public required WeaponTypeValueCollection GrowCurves { get; init; }

    public NameDescriptions? Affix { get; init; }

    public required ImmutableArray<MaterialId> CultivationItems { get; init; }

    [JsonIgnore]
    public WeaponCollocationView? CollocationView { get; set; }

    [JsonIgnore]
    public List<Material>? CultivationItemsView { get; set; }

    [JsonIgnore]
    public QualityType Quality
    {
        get => RankLevel;
    }

    internal uint MaxLevel { get => GetMaxLevelByQuality(Quality); }

    public static uint GetMaxLevelByQuality(QualityType quality)
    {
        return quality >= QualityType.QUALITY_BLUE ? 90U : 70U;
    }

    public TItem ToItem<TItem>()
        where TItem : Model.Item, new()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Quality = RankLevel,
        };
    }

    public StatisticsItem ToStatisticsItem(int count)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Quality = RankLevel,
            Count = count,
        };
    }

    public SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = EquipIconConverter.IconNameToUri(Icon),
            Badge = WeaponTypeIconConverter.WeaponTypeToIconUri(WeaponType),
            Time = time,
            Quality = RankLevel,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }
}