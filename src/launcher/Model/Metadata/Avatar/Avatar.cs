//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata.Abstraction;
using Launcher.Model.Metadata.Converter;
using Launcher.Model.Metadata.Item;
using Launcher.Model.Primitive;
using Launcher.UI.Xaml.Data;
using Launcher.ViewModel.Complex;
using Launcher.ViewModel.GachaLog;
using Launcher.ViewModel.Wiki;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Launcher.Model.Metadata.Avatar;

[DebuggerDisplay("Name={Name},Id={Id}")]
internal partial class Avatar : IDefaultIdentity<AvatarId>,
    INameQualityAccess,
    IStatisticsItemConvertible,
    ISummaryItemConvertible,
    IItemConvertible,
    IPropertyValuesProvider,
    IJsonOnDeserialized
{
    public required AvatarId Id { get; init; }

    public required PromoteId PromoteId { get; init; }

    public required uint Sort { get; init; }

    public required BodyType Body { get; init; }

    public required string Icon { get; init; }

    public required string SideIcon { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required DateTimeOffset BeginTime { get; init; }

    public required QualityType Quality { get; init; }

    public required WeaponType Weapon { get; init; }

    public required AvatarBaseValue BaseValue { get; init; }

    public required TypeValueCollection<FightProperty, GrowCurveType> GrowCurves { get; init; }

    public required SkillDepot SkillDepot { get; init; }

    public required FetterInfo FetterInfo { get; init; }

    public required ImmutableArray<Costume> Costumes { get; init; }

    public required ImmutableArray<MaterialId> CultivationItems { get; init; }

    public required AvatarNameCard NameCard { get; init; }

    public TraceEffect? TraceEffect { get; init; }

    [JsonIgnore]
    public AvatarCollocationView? CollocationView { get; set; }

    [JsonIgnore]
    public CookBonusView? CookBonusView { get; set; }

    [JsonIgnore]
    public ImmutableArray<Material>? CultivationItemsView { get; set; }

    [JsonIgnore]
    public IAdvancedCollectionView<Costume>? CostumesView { get; set; }

    [SuppressMessage("", "CA1822")]
    public uint MaxLevel { get => GetMaxLevel(); }

    public static uint GetMaxLevel()
    {
        return 100U;
    }

    public TItem ToItem<TItem>()
        where TItem : Model.Item, new()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToUri(FetterInfo.VisionBefore),
            Quality = Quality,
        };
    }

    public StatisticsItem ToStatisticsItem(int count)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Count = count,
        };
    }

    public SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Time = time,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }

    public void OnDeserialized()
    {
        if (AvatarIds.UsesGnosis(Id))
        {
            FetterInfo.VisionOverride = SH.ViewPageWiKiAvatarGnosisTitle;
        }
    }
}
