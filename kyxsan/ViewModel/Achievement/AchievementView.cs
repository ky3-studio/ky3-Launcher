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
using kyxsan.Model.Intrinsic;
using kyxsan.UI.Xaml.Data;

namespace kyxsan.ViewModel.Achievement;

internal sealed partial class AchievementView : ObservableObject,
    IEntityAccessWithMetadata<Model.Entity.Achievement, Model.Metadata.Achievement.Achievement>,
    IPropertyValuesProvider
{
    // ReSharper disable once ReplaceWithFieldKeyword
    private bool isChecked;

    private AchievementView(Model.Entity.Achievement entity, Model.Metadata.Achievement.Achievement inner)
    {
        Entity = entity;
        Inner = inner;

        isChecked = entity.Status >= AchievementStatus.STATUS_FINISHED;
    }

    public Model.Entity.Achievement Entity { get; }

    public Model.Metadata.Achievement.Achievement Inner { get; }

    public uint Order { get => Inner.Order; }

    public bool IsChecked
    {
        get => isChecked;
        set
        {
            if (SetProperty(ref isChecked, value))
            {
                (Entity.Status, Entity.Time) = value
                    ? (AchievementStatus.STATUS_REWARD_TAKEN, DateTimeOffset.UtcNow)
                    : (AchievementStatus.STATUS_INVALID, default);

                OnPropertyChanged(nameof(Time));
            }
        }
    }

    public string Time { get => $"{Entity.Time.ToLocalTime():yyyy.MM.dd HH:mm:ss}"; }

    public static AchievementView Create(Model.Entity.Achievement entity, Model.Metadata.Achievement.Achievement inner)
    {
        return new(entity, inner);
    }
}