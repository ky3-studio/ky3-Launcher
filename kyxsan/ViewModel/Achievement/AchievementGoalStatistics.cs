//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using BindingAchievementGoal = kyxsan.ViewModel.Achievement.AchievementGoalView;

namespace kyxsan.ViewModel.Achievement;

internal sealed class AchievementGoalStatistics
{
    private AchievementGoalStatistics(BindingAchievementGoal goal)
    {
        AchievementGoal = goal;
    }

    public BindingAchievementGoal AchievementGoal { get; }

    public int Finished { get; set; }

    public int TotalCount { get; set; }

    public static AchievementGoalStatistics Create(BindingAchievementGoal goal)
    {
        return new(goal);
    }
}