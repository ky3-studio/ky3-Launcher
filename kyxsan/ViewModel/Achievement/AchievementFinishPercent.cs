//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.ViewModel.Achievement;

internal static class AchievementFinishPercent
{
    public static void Update(AchievementViewModel viewModel)
    {
        int totalFinished = 0;
        int totalCount = 0;

        if (viewModel.Achievements is not { } achievements)
        {
            return;
        }

        if (viewModel.AchievementGoals is not { } achievementGoals)
        {
            return;
        }

        Dictionary<AchievementGoalId, AchievementGoalStatistics> counter = achievementGoals.Source.ToDictionary(x => x.Id, AchievementGoalStatistics.Create);

        foreach (AchievementView achievementView in achievements.Source)
        {
            ref AchievementGoalStatistics goalStats = ref CollectionsMarshal.GetValueRefOrNullRef(counter, achievementView.Inner.Goal);
            Debug.Assert(!Unsafe.IsNullRef(in goalStats));

            goalStats.TotalCount += 1;
            totalCount += 1;
            if (achievementView.IsChecked)
            {
                goalStats.Finished += 1;
                totalFinished += 1;
            }
        }

        foreach (AchievementGoalStatistics statistics in counter.Values)
        {
            statistics.AchievementGoal.UpdateFinishDescriptionAndPercent(statistics);
        }

        viewModel.FinishDescription = AchievementStatistics.Format(totalFinished, totalCount, out _);
    }
}