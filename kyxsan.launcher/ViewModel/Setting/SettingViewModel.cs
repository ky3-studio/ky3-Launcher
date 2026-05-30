//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core;
using kyxsan.Core.Logging;
using kyxsan.Core.Shell;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.Navigation;
using kyxsan.Service.Notification;
using kyxsan.Service.Update;
using kyxsan.Service;
using kyxsan.UI.Xaml.View.Dialog;

namespace kyxsan.ViewModel.Setting;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel, INavigationRecipient
{
    public const string UIGFImportExport = nameof(UIGFImportExport);

    private readonly IShellLinkInterop shellLinkInterop;
    private readonly IUpdateService updateService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;
    private readonly AutoStartService autoStartService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;

    private readonly WeakReference<ScrollViewer> weakScrollViewer = new(default!);

    private AppOptions? appOptions;

    [GeneratedConstructor]
    public partial SettingViewModel(IServiceProvider serviceProvider);

    public partial SettingGeetestViewModel Geetest { get; }

    public partial SettingAppearanceViewModel Appearance { get; }

    public partial SettingStorageViewModel Storage { get; }

    public partial SettingHotKeyViewModel HotKey { get; }

    public partial SettingHomeViewModel Home { get; }

    public partial SettingGameViewModel Game { get; }

    [ObservableProperty]
    public partial string? UpdateInfo { get; set; }

    public bool RunElevated
    {
        get => appOptions?.RunElevated?.Value ?? false;
        set
        {
            if (appOptions is null)
            {
                return;
            }

            if (appOptions.RunElevated.Value == value)
            {
                return;
            }

            appOptions.RunElevated.Value = value;
            OnPropertyChanged(nameof(RunElevated));
        }
    }

    public bool IsStartupEnabled
    {
        get => appOptions?.IsStartupEnabled?.Value ?? false;
        set
        {
            if (appOptions is null)
            {
                return;
            }

            if (appOptions.IsStartupEnabled.Value == value)
            {
                return;
            }

            appOptions.IsStartupEnabled.Value = value;
            OnPropertyChanged(nameof(IsStartupEnabled));
        }
    }

    public void AttachXamlElement(ScrollViewer scrollViewer)
    {
        weakScrollViewer.SetTarget(scrollViewer);
    }

    public async ValueTask<bool> ReceiveAsync(INavigationExtraData data, CancellationToken token)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (!weakScrollViewer.TryGetTarget(out ScrollViewer? scrollViewer))
        {
            return false;
        }

        return false;
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        MakeSubViewModel([Geetest, Appearance, Storage, HotKey, Home, Game]);

        Storage.CacheFolderView = new(taskContext, kyxsanRuntime.LocalCacheDirectory);
        Storage.DataFolderView = new(taskContext, kyxsanRuntime.DataDirectory);

        UpdateInfo = updateService.UpdateInfo;

        try
        {
            bool startup = autoStartService.IsStartupEnabled();
            bool runElevated = autoStartService.IsRunElevatedEnabled();
            AppOptions options = Ioc.Default.GetRequiredService<AppOptions>();
            appOptions = options;

            options.RunElevated.PropertyChanged += (s, e) =>
            {
                try
                {
                    taskContext.InvokeOnMainThread(() => OnPropertyChanged(nameof(RunElevated)));
                }
                catch
                {
                }
            };

            options.IsStartupEnabled.PropertyChanged += (s, e) =>
            {
                try
                {
                    taskContext.InvokeOnMainThread(() => OnPropertyChanged(nameof(IsStartupEnabled)));
                }
                catch
                {
                }
            };

            OnPropertyChanged(nameof(RunElevated));
            OnPropertyChanged(nameof(IsStartupEnabled));
        }
        catch
        {
        }

        return true;
    }

    [Command("SubmitFeedbackCommand")]
    private async Task SubmitFeedbackAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Submit feedback", "SettingViewModel.Command"));
        SubmitFeedbackDialog dialog = await contentDialogFactory.CreateInstanceAsync<SubmitFeedbackDialog>(serviceProvider).ConfigureAwait(false);
        await dialog.ShowFeedbackAsync().ConfigureAwait(false);
    }

    [Command("CheckUpdateCommand")]
    private async Task CheckUpdateAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Check update", "SettingViewModel.Command"));

        await taskContext.SwitchToBackgroundAsync();

        CheckUpdateResult result = await updateService.CheckUpdateAsync().ConfigureAwait(false);
        await taskContext.InvokeOnMainThreadAsync(() => UpdateInfo = result.Kind switch
        {
            CheckUpdateResultKind.UpdateAvailable => SH.FormatViewModelSettingUpdateAvailable(result.PackageInformation?.Version.ToString()),
            CheckUpdateResultKind.AlreadyUpdated => SH.ViewModelSettingAlreadyUpdated,
            CheckUpdateResultKind.VersionApiInvalidResponse or CheckUpdateResultKind.VersionApiInvalidSha256 => SH.ViewModelSettingCheckUpdateFailed,
            _ => default!,
        }).ConfigureAwait(false);

        await updateService.TriggerUpdateAsync(result).ConfigureAwait(false);
    }

    [Command("RestartAsElevatedCommand")]
    private void RestartAsElevated()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Restart as elevated", "NotifyIconViewModel.Command"));
        NativeMethods.RestartAsAdministrator();
    }

    [Command("CreateDesktopShortcutCommand")]
    private void CreateDesktopShortcutForElevatedLaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Create desktop shortcut for elevated launch", "SettingViewModel.Command"));

        _ = shellLinkInterop.TryCreateDesktopShortcutForElevatedLaunch()
            ? messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingActionComplete))
            : messenger.Send(InfoBarMessage.Warning(SH.ViewModelSettingCreateDesktopShortcutFailed));
    }

}