//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.IO;
using Launcher.Core.Logging;
using Launcher.Core.Setting;
using Launcher.Factory.ContentDialog;
using Launcher.Factory.Picker;
using Launcher.Factory.Process;
using Launcher.Service.Game.AdvancedStart.Model;
using Launcher.Service.Notification;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.UI.Xaml.View.Window;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace Launcher.ViewModel.Game;

internal sealed partial class LaunchGameViewModel
{
    public ObservableCollection<AdvancedStartDelayedProgramEntry> Entries { get; private set => SetProperty(ref field, value); } = [];

    private AdvancedStartDelayedProgramEntry? selectedDelayedProgramEntry;

    public AdvancedStartDelayedProgramEntry? SelectedDelayedProgramEntry
    {
        get => selectedDelayedProgramEntry;
        set => SetProperty(ref selectedDelayedProgramEntry, value);
    }

    private void WireEntries(ObservableCollection<AdvancedStartDelayedProgramEntry> entries)
    {
        entries.CollectionChanged += Entries_CollectionChanged;

        foreach (AdvancedStartDelayedProgramEntry entry in entries)
        {
            entry.PropertyChanged += Entry_PropertyChanged;
        }
    }

    private void Entries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (object item in e.NewItems)
            {
                if (item is AdvancedStartDelayedProgramEntry added)
                {
                    added.PropertyChanged += Entry_PropertyChanged;
                }
            }
        }

        if (e.OldItems is not null)
        {
            foreach (object item in e.OldItems)
            {
                if (item is AdvancedStartDelayedProgramEntry removed)
                {
                    removed.PropertyChanged -= Entry_PropertyChanged;
                }
            }
        }

        ScheduleDelayedSave();
    }

    private void Entry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ScheduleDelayedSave();
    }

    private void ScheduleDelayedSave()
    {
        CancellationTokenSource? previous;

        lock (delayedSaveGate)
        {
            previous = delayedSaveCts;
            delayedSaveCts = new CancellationTokenSource();
        }

        previous?.Cancel();

        CancellationToken token = delayedSaveCts!.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), token).ConfigureAwait(false);
                store.Save(Entries);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                messenger.Send(InfoBarMessage.Error(ex));
            }
        });
    }

    [Command("IdentifyMonitorsCommand")]
    private static async Task IdentifyMonitorsAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Identify monitors", "LaunchGameViewModel.Command"));
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }

    [Command("LaunchAdvancedCommand")]
    private async Task LaunchAdvancedAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch advanced start program", "LaunchGameViewModel.Command"));

        try
        {
            string programPath = LocalSetting.Get(SettingKeys.LaunchAdvancedStartProgramPath, string.Empty);
            if (string.IsNullOrWhiteSpace(programPath))
            {
                messenger.Send(InfoBarMessage.Warning(SH.ViewModelLaunchGameAdvancedStartProgramPathNotSet));
                return;
            }

            if (!File.Exists(programPath))
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameAdvancedStartProgramNotExists, programPath));
                return;
            }

            ProcessFactory.StartUsingShellExecute(string.Empty, programPath);
            messenger.Send(InfoBarMessage.Success(SH.ViewModelLaunchGameAdvancedStartProgramLaunched));

            if (LaunchOptions.AdvancedStartDelayedOnAdvancedStart.Value)
            {
                Shared.LaunchAdvancedDelayedAsync().SafeForget();
            }
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("LaunchAdvancedDelayedCommand")]
    private Task LaunchAdvancedDelayedCommandAsync()
    {
        Shared.LaunchAdvancedDelayedAsync(CancellationToken).SafeForget();
        return Task.CompletedTask;
    }

    [Command("PickAdvancedStartProgramPathCommand")]
    private async Task PickAdvancedStartProgramPathAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Pick advanced start program", "LaunchGameViewModel.Command"));

        await taskContext.SwitchToBackgroundAsync();
        (bool picked, ValueFile file) = fileSystemPickerInteraction.PickFile(
            "Picker",
            "program",
            "*.exe");

        if (!picked)
        {
            return;
        }

        string path = file;

        LocalSetting.Set(SettingKeys.LaunchAdvancedStartProgramPath, path);

        await taskContext.SwitchToMainThreadAsync();
        AdvancedStartProgramPath = path;
        messenger.Send(InfoBarMessage.Success(SH.ViewModelLaunchGameAdvancedStartProgramPathSaved));
    }

    [Command("OpenAdvancedStartCommunityCommand")]
    private static async Task OpenAdvancedStartCommunityAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open advanced start community", "LaunchGameViewModel.Command"));

        await Windows.System.Launcher.LaunchUriAsync("about:blank".ToUri());
    }

    [Command("OpenAdvancedStartCheckDownloadCommand")]
    private async Task OpenAdvancedStartCheckDownloadAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open advanced start downloader", "LaunchGameViewModel.Command"));

        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IContentDialogFactory dialogFactory = scope.ServiceProvider.GetRequiredService<IContentDialogFactory>();
                LaunchGameAdvancedStartDownloadDialog dialog = await dialogFactory.CreateInstanceAsync<LaunchGameAdvancedStartDownloadDialog>(scope.ServiceProvider).ConfigureAwait(false);

                (bool ok, string? programPath) = await dialog.GetResultAsync().ConfigureAwait(false);
                if (!ok || string.IsNullOrWhiteSpace(programPath))
                {
                    return;
                }

                LocalSetting.Set(SettingKeys.LaunchAdvancedStartProgramPath, programPath);
                await taskContext.SwitchToMainThreadAsync();
                AdvancedStartProgramPath = programPath;
                messenger.Send(InfoBarMessage.Success(SH.ViewModelLaunchGameAdvancedStartProgramPathSaved));
            }
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("OpenAdvancedStartListSourceSetterCommand")]
    private async Task OpenAdvancedStartListSourceSetterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open advanced start LaunchGameAdvancedStartDownloaderSourceDialog", "LaunchGameViewModel.Command"));

        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IContentDialogFactory dialogFactory = scope.ServiceProvider.GetRequiredService<IContentDialogFactory>();
                LaunchGameAdvancedStartDownloaderSourceDialog dialog = await dialogFactory.CreateInstanceAsync<LaunchGameAdvancedStartDownloaderSourceDialog>(scope.ServiceProvider).ConfigureAwait(false);

                (bool ok, string? programPath) = await dialog.GetResultAsync().ConfigureAwait(false);
                if (!ok || string.IsNullOrWhiteSpace(programPath))
                {
                    return;
                }

                LocalSetting.Set(SettingKeys.LaunchAdvancedStartProgramPath, programPath);
                await taskContext.SwitchToMainThreadAsync();
                AdvancedStartProgramPath = programPath;
                messenger.Send(InfoBarMessage.Success(SH.ViewModelLaunchGameAdvancedStartProgramPathSaved));
            }
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("AddDelayedProgramCommand")]
    private async Task AddDelayedProgramAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        string path = file;
        string name = ExecutableInfoHelper.GetFriendlyName(path);

        await taskContext.SwitchToMainThreadAsync();
        AdvancedStartDelayedProgramEntry entry = new(name, path, 0);
        Entries.Add(entry);
        SelectedDelayedProgramEntry = entry;
        store.Save(Entries);
    }

    [Command("RemoveDelayedProgramCommand")]
    private void RemoveDelayedProgram()
    {
        if (SelectedDelayedProgramEntry is null)
        {
            return;
        }

        Entries.Remove(SelectedDelayedProgramEntry);
        SelectedDelayedProgramEntry = null;
        store.Save(Entries);
    }

    [Command("SaveDelayedProgramCommand")]
    private void SaveDelayedProgram()
    {
        store.Save(Entries);
        messenger.Send(InfoBarMessage.Success(SH.ViewModelLaunchGameAdvancedStartProgramPathSaved));
    }

    [Command("EditDelayedProgramCommand")]
    private Task EditDelayedProgramAsync()
    {
        return PickDelayedProgramPathAsync(SelectedDelayedProgramEntry);
    }

    [Command("PickDelayedProgramPathCommand")]
    private async Task PickDelayedProgramPathAsync(AdvancedStartDelayedProgramEntry? entry)
    {
        if (entry is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        entry.Path = file;
        if (string.IsNullOrWhiteSpace(entry.Name))
        {
            entry.Name = Path.GetFileNameWithoutExtension(entry.Path);
        }

        store.Save(Entries);
    }
}
