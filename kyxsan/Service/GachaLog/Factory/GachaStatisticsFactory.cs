//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.UI.Xaml.Data;
using kyxsan.ViewModel.GachaLog;
using System.Collections.Immutable;
using GachaItem = kyxsan.Model.Entity.GachaItem;

namespace kyxsan.Service.GachaLog.Factory;

[Service(ServiceLifetime.Singleton, typeof(IGachaStatisticsFactory))]
internal sealed partial class GachaStatisticsFactory : IGachaStatisticsFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial GachaStatisticsFactory(IServiceProvider serviceProvider);

    public async ValueTask<GachaStatistics> CreateAsync(GachaLogServiceMetadataContext metadata, ImmutableArray<GachaItem> items)
    {
        await taskContext.SwitchToBackgroundAsync();

        ImmutableArray<HistoryWishBuilder> historyWishBuilders = metadata.GachaEvents
            .SelectAsArray(static (gachaEvent, metadata) => HistoryWishBuilder.Create(gachaEvent, metadata), metadata);
        return SynchronizedCreate(new(serviceProvider, items, historyWishBuilders, metadata));
    }

    private static GachaStatistics SynchronizedCreate(GachaStatisticsFactoryContext context)
    {
        TypedWishSummaryBuilder standardWishBuilder = TypedWishSummaryBuilderContext.StandardWish(context).CreateBuilder();
        TypedWishSummaryBuilder avatarWishBuilder = TypedWishSummaryBuilderContext.AvatarEventWish(context).CreateBuilder();
        TypedWishSummaryBuilder weaponWishBuilder = TypedWishSummaryBuilderContext.WeaponEventWish(context).CreateBuilder();
        TypedWishSummaryBuilder chronicledWishBuilder = TypedWishSummaryBuilderContext.ChronicledWish(context).CreateBuilder();
        GachaStatisticsItemCounter itemCounter = new(context);

        Dictionary<GachaType, List<HistoryWishBuilder>> historyWishBuilderMap = context.HistoryWishBuilders
            .GroupBy(b => b.ConfigType)
            .ToDictionary(g => g.Key, g => g.ToList().SortBy(b => b.From));

        foreach (ref readonly GachaItem item in context.Items.AsSpan())
        {
            HistoryWishBuilder? targetHistoryWishBuilder = item.GachaType is not (GachaType.Standard or GachaType.Novice)
                && historyWishBuilderMap.TryGetValue(item.GachaType, out List<HistoryWishBuilder>? builders)
                ? builders.BinarySearch(item, static (pinned, banner) => pinned.Time < banner.From ? -1 : pinned.Time > banner.To ? 1 : 0)
                : default;

            switch (item.ItemId.StringLength)
            {
                case 8U:
                    {
                        Avatar avatar = context.Metadata.GetAvatar(item.ItemId);

                        bool isUp = false;
                        switch (avatar.Quality)
                        {
                            case QualityType.QUALITY_ORANGE:
                                itemCounter.OrangeAvatar.IncreaseByOne(avatar);
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(avatar) ?? false;
                                if (!isUp && item.GachaType is GachaType.AvatarEvent or GachaType.AvatarEvent2 && !AvatarIds.IsStandardWish(avatar.Id))
                                {
                                    isUp = true;
                                }

                                break;
                            case QualityType.QUALITY_PURPLE:
                                itemCounter.PurpleAvatar.IncreaseByOne(avatar);
                                targetHistoryWishBuilder?.IncreasePurple(avatar);
                                break;
                        }

                        standardWishBuilder.Track(item, avatar, isUp);
                        avatarWishBuilder.Track(item, avatar, isUp);
                        weaponWishBuilder.Track(item, avatar, isUp);
                        chronicledWishBuilder.Track(item, avatar, isUp);
                        break;
                    }

                case 5U:
                    {
                        Weapon weapon = context.Metadata.GetWeapon(item.ItemId);

                        bool isUp = false;
                        switch (weapon.RankLevel)
                        {
                            case QualityType.QUALITY_ORANGE:
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(weapon) ?? false;
                                if (!isUp && item.GachaType is GachaType.WeaponEvent && !WeaponIds.IsOrangeStandardWish(weapon.Id))
                                {
                                    isUp = true;
                                }

                                itemCounter.OrangeWeapon.IncreaseByOne(weapon);
                                break;
                            case QualityType.QUALITY_PURPLE:
                                targetHistoryWishBuilder?.IncreasePurple(weapon);
                                itemCounter.PurpleWeapon.IncreaseByOne(weapon);
                                break;
                            case QualityType.QUALITY_BLUE:
                                targetHistoryWishBuilder?.IncreaseBlue(weapon);
                                itemCounter.BlueWeapon.IncreaseByOne(weapon);
                                break;
                        }

                        standardWishBuilder.Track(item, weapon, isUp);
                        avatarWishBuilder.Track(item, weapon, isUp);
                        weaponWishBuilder.Track(item, weapon, isUp);
                        chronicledWishBuilder.Track(item, weapon, isUp);
                        break;
                    }

                default:
                    break;
            }
        }

        AsyncBarrier barrier = new(4);

        ImmutableArray<HistoryWish> historyWishes =
        [
            .. context.HistoryWishBuilders
                .Where(b => context.IsEmptyHistoryWishVisible || !b.IsEmpty)
                .OrderByDescending(builder => builder.From)
                .ThenBy(builder => builder.ConfigType, GachaTypeComparer.Shared)
                .Select(builder => builder.ToHistoryWish())
        ];

        return new()
        {
            HistoryWishes = historyWishes.AsAdvancedCollectionView(),

            OrangeAvatars = itemCounter.OrangeAvatar.ToStatisticsImmutableArray(),
            PurpleAvatars = itemCounter.PurpleAvatar.ToStatisticsImmutableArray(),

            OrangeWeapons = itemCounter.OrangeWeapon.ToStatisticsImmutableArray(),
            PurpleWeapons = itemCounter.PurpleWeapon.ToStatisticsImmutableArray(),
            BlueWeapons = itemCounter.BlueWeapon.ToStatisticsImmutableArray(),

            StandardWish = standardWishBuilder.ToTypedWishSummary(barrier),
            AvatarWish = avatarWishBuilder.ToTypedWishSummary(barrier),
            WeaponWish = weaponWishBuilder.ToTypedWishSummary(barrier),
            ChronicledWish = chronicledWishBuilder.ToTypedWishSummary(barrier),
        };
    }
}
