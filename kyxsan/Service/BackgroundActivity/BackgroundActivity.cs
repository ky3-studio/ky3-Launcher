//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace kyxsan.Service.BackgroundActivity;

internal sealed partial class BackgroundActivity : ObservableObject
{
    public BackgroundActivity(string title, string description)
    {
        Title = title;
        Description = description;
    }

    [ObservableProperty]
    public partial string Title { get; private set; }

    [ObservableProperty]
    public partial string Description { get; private set; }

    [ObservableProperty]
    public partial bool IsCompletedSuccessfully { get; private set; } = true;

    [ObservableProperty]
    public partial bool IsFaulted { get; private set; }

    [ObservableProperty]
    public partial bool HasProgress { get; private set; }

    [ObservableProperty]
    public partial bool IsIndeterminate { get; private set; }

    [ObservableProperty]
    public partial double ProgressValue { get; private set; }

    [ObservableProperty]
    public partial int NotifyToken { get; private set; }

    public async ValueTask UpdateAsync(ITaskContext taskContext, string description, bool isCompletedSuccessfully, bool isFaulted, bool hasProgress, bool isIndeterminate)
    {
        await taskContext.SwitchToMainThreadAsync();

        Description = description;
        IsCompletedSuccessfully = isCompletedSuccessfully;
        HasProgress = hasProgress;
        IsIndeterminate = isIndeterminate;
    }

    public async ValueTask UpdateAsync(ITaskContext taskContext, string description, bool isCompletedSuccessfully, bool isFaulted, bool hasProgress, bool isIndeterminate, double progressValue)
    {
        await taskContext.SwitchToMainThreadAsync();

        Description = description;
        IsCompletedSuccessfully = isCompletedSuccessfully;
        HasProgress = hasProgress;
        IsIndeterminate = isIndeterminate;
        ProgressValue = progressValue;
    }

    public void Update(ITaskContext taskContext, string description, bool isCompletedSuccessfully, bool isFaulted, bool hasProgress, bool isIndeterminate)
    {
        taskContext.InvokeOnMainThread(() =>
        {
            Description = description;
            IsCompletedSuccessfully = isCompletedSuccessfully;
            HasProgress = hasProgress;
            IsIndeterminate = isIndeterminate;
        });
    }

    public void Update(ITaskContext taskContext, string description, bool isCompletedSuccessfully, bool isFaulted, bool hasProgress, bool isIndeterminate, double progressValue)
    {
        taskContext.InvokeOnMainThread(() =>
        {
            Description = description;
            IsCompletedSuccessfully = isCompletedSuccessfully;
            HasProgress = hasProgress;
            IsIndeterminate = isIndeterminate;
            ProgressValue = progressValue;
        });
    }

    public async ValueTask NotifyAsync(ITaskContext taskContext)
    {
        await taskContext.SwitchToMainThreadAsync();
        NotifyToken++;
    }
}