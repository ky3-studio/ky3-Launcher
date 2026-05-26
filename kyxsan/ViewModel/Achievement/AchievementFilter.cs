//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using kyxsan.UI.Xaml.Data;
using System.Text.RegularExpressions;

namespace kyxsan.ViewModel.Achievement;

internal static partial class AchievementFilter
{
    [GeneratedRegex(@"\p{P}")]
    private static partial Regex PunctuationRegex { get; }

    public static Predicate<AchievementGoalView>? GoalCompile(IAdvancedCollectionView<AchievementView> collection)
    {
        if (collection.Filter is null)
        {
            return default;
        }

        return goal => collection.View.Any(view => view.Inner.Goal == goal.Id);
    }

    public static Predicate<AchievementView>? Compile(bool filterDailyQuest)
    {
        return view => DailyQuestComponent(view, filterDailyQuest);
    }

    public static Predicate<AchievementView>? Compile(bool filterDailyQuest, AchievementGoalView? goal)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && GoalComponent(view, goal);
    }

    public static Predicate<AchievementView>? Compile(bool filterDailyQuest, AchievementId achievementId)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && AchievementIdComponent(view, achievementId);
    }

    public static Predicate<AchievementView>? CompileForVersion(bool filterDailyQuest, string version)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && VersionComponent(view, version);
    }

    public static Predicate<AchievementView>? CompileForTitleOrDescription(bool filterDailyQuest, string search)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && SearchComponent(view, search);
    }

    private static bool DailyQuestComponent(AchievementView view, bool filterDailyQuest)
    {
        return !filterDailyQuest || view.Inner.IsDailyQuest;
    }

    private static bool GoalComponent(AchievementView view, AchievementGoalView? goal)
    {
        return goal is null || view.Inner.Goal == goal.Id;
    }

    private static bool AchievementIdComponent(AchievementView view, AchievementId achievementId)
    {
        return view.Inner.Id == achievementId;
    }

    private static bool VersionComponent(AchievementView view, string version)
    {
        return string.Equals(view.Inner.Version, version, StringComparison.OrdinalIgnoreCase);
    }

    private static bool SearchComponent(AchievementView view, string search)
    {
        return PunctuationRegex.Replace(view.Inner.Title, string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)
            || view.Inner.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase)
            || PunctuationRegex.Replace(view.Inner.Description, string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)
            || view.Inner.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase);
    }
}