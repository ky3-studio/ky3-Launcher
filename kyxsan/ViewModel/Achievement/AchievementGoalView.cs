//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model;
using kyxsan.Model.Metadata.Achievement;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Primitive;
using kyxsan.UI.Xaml.Data;

namespace kyxsan.ViewModel.Achievement;

internal sealed partial class AchievementGoalView : ObservableObject,
    INameIcon<Uri>,
    IPropertyValuesProvider
{
    private AchievementGoalView(AchievementGoal goal)
    {
        Id = goal.Id;
        Order = goal.Order;
        Name = goal.Name;
        Icon = AchievementIconConverter.IconNameToUri(goal.Icon);
    }

    public AchievementGoalId Id { get; }

    public uint Order { get; }

    public string Name { get; }

    public Uri Icon { get; }

    [ObservableProperty]
    public partial double FinishPercent { get; private set; }

    [ObservableProperty]
    public partial string? FinishDescription { get; private set; }

    public static AchievementGoalView Create(AchievementGoal source)
    {
        return new(source);
    }

    public void UpdateFinishDescriptionAndPercent(AchievementGoalStatistics statistics)
    {
        FinishDescription = AchievementStatistics.Format(statistics.Finished, statistics.TotalCount, out double finishPercent);
        FinishPercent = finishPercent;
    }
}