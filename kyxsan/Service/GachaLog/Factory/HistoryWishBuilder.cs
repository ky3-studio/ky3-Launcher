//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.ViewModel.GachaLog;

namespace kyxsan.Service.GachaLog.Factory;

internal sealed class HistoryWishBuilder
{
    private readonly GachaEvent gachaEvent;

    private readonly Dictionary<IStatisticsItemConvertible, int> orangeUpCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> purpleUpCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> orangeCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> purpleCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> blueCounter = [];

    private int totalCountTracker;

    private HistoryWishBuilder(GachaEvent gachaEvent, GachaLogServiceMetadataContext context)
    {
        this.gachaEvent = gachaEvent;
        ConfigType = gachaEvent.Type;

        switch (ConfigType)
        {
            case GachaType.AvatarEvent:
            case GachaType.AvatarEvent2:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.GetAvatar(id)).ToDictionary(IStatisticsItemConvertible (a) => a, _ => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.GetAvatar(id)).ToDictionary(IStatisticsItemConvertible (a) => a, _ => 0);
                break;
            case GachaType.WeaponEvent:
                orangeUpCounter = gachaEvent.UpOrangeList
                    .Select(id => context.IdWeaponMap.GetValueOrDefault(id))
                    .OfType<IStatisticsItemConvertible>()
                    .ToDictionary(w => w, _ => 0);
                purpleUpCounter = gachaEvent.UpPurpleList
                    .Select(id => context.IdWeaponMap.GetValueOrDefault(id))
                    .OfType<IStatisticsItemConvertible>()
                    .ToDictionary(w => w, _ => 0);
                break;
            case GachaType.Chronicled:
                orangeUpCounter = gachaEvent.UpOrangeList
                    .Select(id => (IStatisticsItemConvertible?)context.IdAvatarMap.GetValueOrDefault(id) ?? context.IdWeaponMap.GetValueOrDefault(id))
                    .OfType<IStatisticsItemConvertible>()
                    .ToDictionary(c => c, _ => 0);
                purpleUpCounter = gachaEvent.UpPurpleList
                    .Select(id => (IStatisticsItemConvertible?)context.IdAvatarMap.GetValueOrDefault(id) ?? context.IdWeaponMap.GetValueOrDefault(id))
                    .OfType<IStatisticsItemConvertible>()
                    .ToDictionary(c => c, _ => 0);
                break;
        }
    }

    public GachaType ConfigType { get; }

    public DateTimeOffset From { get => gachaEvent.From; }

    public DateTimeOffset To { get => gachaEvent.To; }

    public bool IsEmpty { get => totalCountTracker <= 0; }

    public static HistoryWishBuilder Create(GachaEvent gachaEvent, GachaLogServiceMetadataContext context)
    {
        return new(gachaEvent, context);
    }

    public bool IncreaseOrange(IStatisticsItemConvertible item)
    {
        orangeCounter.IncreaseByOne(item);
        ++totalCountTracker;

        return orangeUpCounter.TryIncreaseByOne(item);
    }

    public void IncreasePurple(IStatisticsItemConvertible item)
    {
        purpleUpCounter.TryIncreaseByOne(item);
        purpleCounter.IncreaseByOne(item);
        ++totalCountTracker;
    }

    public void IncreaseBlue(IStatisticsItemConvertible item)
    {
        blueCounter.IncreaseByOne(item);
        ++totalCountTracker;
    }

    public HistoryWish ToHistoryWish()
    {
        HistoryWish historyWish = new()
        {
            Name = gachaEvent.Name,
            From = gachaEvent.From,
            To = gachaEvent.To,
            TotalCount = totalCountTracker,

            Version = gachaEvent.Version,
            BannerImage = gachaEvent.Banner,
            OrangeUpList = orangeUpCounter.ToStatisticsImmutableArray(),
            PurpleUpList = purpleUpCounter.ToStatisticsImmutableArray(),
            OrangeList = orangeCounter.ToStatisticsImmutableArray(),
            PurpleList = purpleCounter.ToStatisticsImmutableArray(),
            BlueList = blueCounter.ToStatisticsImmutableArray(),
        };

        return historyWish;
    }
}
