//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.ViewModel.GachaLog;

namespace kyxsan.Service.GachaLog.Factory;

internal sealed class TypedWishSummaryBuilder
{
    private readonly TypedWishSummaryBuilderContext context;

    private readonly List<int> averageOrangePullTracker = [];
    private readonly List<int> averageUpOrangePullTracker = [];
    private readonly List<SummaryItem> summaryItems = [];

    private int maxOrangePullTracker;
    private int minOrangePullTracker;
    private int lastOrangePullTracker;
    private int lastUpOrangePullTracker;
    private int lastPurplePullTracker;
    private int totalCountTracker;
    private int totalOrangePullTracker;
    private int totalPurplePullTracker;
    private int totalBluePullTracker;

    private DateTimeOffset fromTimeTracker = DateTimeOffset.MaxValue;
    private DateTimeOffset toTimeTracker = DateTimeOffset.MinValue;

    public TypedWishSummaryBuilder(TypedWishSummaryBuilderContext context)
    {
        this.context = context;
    }

    public void Track(GachaItem item, ISummaryItemConvertible source, bool isUp)
    {
        if (!context.TypeEvaluator(item.GachaType))
        {
            return;
        }

        ++lastOrangePullTracker;
        ++lastPurplePullTracker;
        ++lastUpOrangePullTracker;

        ++totalCountTracker;
        TrackFromToTime(item.Time);

        switch (source.Quality)
        {
            case QualityType.QUALITY_ORANGE:
                {
                    TrackMinMaxOrangePull(lastOrangePullTracker);
                    averageOrangePullTracker.Add(lastOrangePullTracker);

                    if (isUp)
                    {
                        averageUpOrangePullTracker.Add(lastUpOrangePullTracker);
                        lastUpOrangePullTracker = 0;
                    }

                    summaryItems.Add(source.ToSummaryItem(lastOrangePullTracker, item.Time, isUp));

                    lastOrangePullTracker = 0;
                    ++totalOrangePullTracker;
                    break;
                }

            case QualityType.QUALITY_PURPLE:
                {
                    lastPurplePullTracker = 0;
                    ++totalPurplePullTracker;
                    break;
                }

            case QualityType.QUALITY_BLUE:
                {
                    ++totalBluePullTracker;
                    break;
                }
        }
    }

    public TypedWishSummary ToTypedWishSummary(AsyncBarrier barrier)
    {
        bool isEventBanner = context.DistributionType is GachaDistributionType.AvatarEvent or GachaDistributionType.WeaponEvent;
        summaryItems.CompleteAdding(context.GuaranteeOrangeThreshold, isEventBanner);
        double totalCount = totalCountTracker;

        TypedWishSummary summary = new()
        {
            Name = context.Name,
            TypeName = $"{context.DistributionType:D}",
            From = fromTimeTracker,
            To = toTimeTracker,
            TotalCount = totalCountTracker,

            MaxOrangePull = maxOrangePullTracker,
            MinOrangePull = minOrangePullTracker,
            LastOrangePull = lastOrangePullTracker,
            GuaranteeOrangeThreshold = context.GuaranteeOrangeThreshold,
            LastPurplePull = lastPurplePullTracker,
            GuaranteePurpleThreshold = context.GuaranteePurpleThreshold,
            TotalOrangePull = totalOrangePullTracker,
            TotalPurplePull = totalPurplePullTracker,
            TotalBluePull = totalBluePullTracker,
            TotalOrangePercent = totalOrangePullTracker / totalCount,
            TotalPurplePercent = totalPurplePullTracker / totalCount,
            TotalBluePercent = totalBluePullTracker / totalCount,
            AverageOrangePull = averageOrangePullTracker.Average(),
            AverageUpOrangePull = averageUpOrangePullTracker.Average(),
            OrangeList = summaryItems,
        };

        new PullPrediction(summary, context).PredictAsync(barrier).SafeForget();

        return summary;
    }

    private void TrackMinMaxOrangePull(int lastOrangePull)
    {
        if (lastOrangePull < minOrangePullTracker || minOrangePullTracker == 0)
        {
            minOrangePullTracker = lastOrangePull;
        }

        if (lastOrangePull > maxOrangePullTracker || maxOrangePullTracker == 0)
        {
            maxOrangePullTracker = lastOrangePull;
        }
    }

    private void TrackFromToTime(in DateTimeOffset time)
    {
        if (time < fromTimeTracker)
        {
            fromTimeTracker = time;
        }

        if (time > toTimeTracker)
        {
            toTimeTracker = time;
        }
    }
}
