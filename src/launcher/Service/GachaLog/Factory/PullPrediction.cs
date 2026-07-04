//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.ViewModel.GachaLog;

namespace Launcher.Service.GachaLog.Factory;

internal sealed class PullPrediction
{
    private const double AvatarEventBaseRate = 0.006;
    private const int AvatarEventSoftPityStart = 74;
    private const double AvatarEventRateIncrement = 0.06;

    private const double WeaponEventBaseRate = 0.007;
    private const int WeaponEventSoftPityStart = 63;
    private const double WeaponEventRateIncrement = 0.07;

    private readonly TypedWishSummary typedWishSummary;
    private readonly TypedWishSummaryBuilderContext context;

    public PullPrediction(TypedWishSummary typedWishSummary, in TypedWishSummaryBuilderContext context)
    {
        this.typedWishSummary = typedWishSummary;
        this.context = context;
    }

    [SuppressMessage("", "SH003")]
    public async Task PredictAsync(AsyncBarrier barrier)
    {
        await context.TaskContext.SwitchToBackgroundAsync();

        int currentPity = typedWishSummary.LastOrangePull;
        int hardPity = context.GuaranteeOrangeThreshold;

        (double baseRate, int softPityStart, double rateIncrement) = context.DistributionType switch
        {
            GachaDistributionType.WeaponEvent => (WeaponEventBaseRate, WeaponEventSoftPityStart, WeaponEventRateIncrement),
            _ => (AvatarEventBaseRate, AvatarEventSoftPityStart, AvatarEventRateIncrement),
        };

        double nextPullProb = GetProbabilityAtPull(currentPity + 1, baseRate, softPityStart, rateIncrement, hardPity);

        double survivalProb = 1.0;
        int predictedPulls = 0;
        double predictedProb = 0;

        for (int i = 1; i <= hardPity - currentPity; i++)
        {
            double p = GetProbabilityAtPull(currentPity + i, baseRate, softPityStart, rateIncrement, hardPity);
            survivalProb *= 1.0 - p;
            double cumulative = 1.0 - survivalProb;

            if (predictedPulls == 0 && cumulative >= 0.5)
            {
                predictedPulls = i;
                predictedProb = cumulative;
            }
        }

        if (predictedPulls == 0)
        {
            predictedPulls = hardPity - currentPity;
            predictedProb = 1.0;
        }

        await context.TaskContext.SwitchToMainThreadAsync();
        typedWishSummary.PredictedPullLeftToOrange = predictedPulls;
        typedWishSummary.ProbabilityOfPredictedPullLeftToOrange = predictedProb;
        typedWishSummary.ProbabilityOfNextPullIsOrange = nextPullProb;
        typedWishSummary.IsPredictPullAvailable = true;

        await barrier.SignalAndWaitAsync().ConfigureAwait(false);
    }

    private static double GetProbabilityAtPull(int pull, double baseRate, int softPityStart, double rateIncrement, int hardPity)
    {
        if (pull >= hardPity)
        {
            return 1.0;
        }

        if (pull < softPityStart)
        {
            return baseRate;
        }

        double rate = baseRate + ((pull - softPityStart) + 1) * rateIncrement;
        return Math.Min(1.0, rate);
    }
}
