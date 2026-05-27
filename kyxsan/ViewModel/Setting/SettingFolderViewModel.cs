//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.IO;
using kyxsan.Core.Logging;
using System.IO;
using Windows.System;

namespace kyxsan.ViewModel.Setting;

internal sealed partial class SettingFolderViewModel : ObservableObject
{
    private readonly ITaskContext taskContext;

    public SettingFolderViewModel(ITaskContext taskContext, string folder)
    {
        this.taskContext = taskContext;
        Folder = folder;

        UpdateFolderSizeTimeoutAsync(TimeSpan.FromSeconds(5)).SafeForget();
    }

    public string Folder { get; }

    [ObservableProperty]
    public partial string? Size { get; set; }

    [SuppressMessage("", "SH003")]
    public async Task UpdateFolderSizeTimeoutAsync(TimeSpan timeout)
    {
        // We don't want this function to run indefinitely in principle,
        // users can have a lot of files in the folder if they manually put them in
        using (CancellationTokenSource source = new(timeout))
        {
            try
            {
                await UpdateFolderSizeAsync(source.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    private async ValueTask UpdateFolderSizeAsync(CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        long totalSize = 0;

        if (!Directory.Exists(Folder))
        {
            return;
        }

        totalSize = DirectoryOperation.GetSize(Folder, token);
        await taskContext.SwitchToMainThreadAsync();
        Size = SH.FormatViewModelSettingFolderSizeDescription(Converters.ToFileSizeString(totalSize));
    }

    [Command("OpenFolderCommand")]
    private async Task OpenFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open folder", "SettingFolderViewModel.Command"));
        await Launcher.LaunchFolderPathAsync(Folder);
    }
}