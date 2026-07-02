//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Launcher.Core;
using Launcher.Core.ApplicationModel;
using Launcher.Core.LifeCycle;
using Launcher.Core.Logging;
using Launcher.Core.Shell;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Navigation;
using Launcher.Service.Notification;
using Launcher.Service.Update;
using Launcher.Service;
using Launcher.Service.RemoteConfig;
using Launcher.Core.IO.Http.Proxy;
using Launcher.UI.Xaml.Behavior.Action;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.UI.Xaml.View.Window.WebView2;
using Launcher.Web.Launcher;
using System.Diagnostics;
using System.IO;

namespace Launcher.ViewModel.Setting;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel, INavigationRecipient
{
    public const string UIGFImportExport = nameof(UIGFImportExport);

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IShellLinkInterop shellLinkInterop;
    private readonly IUpdateService updateService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;
    private readonly AutoStartService autoStartService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly AppOptions appOptionsInstance;

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

        Storage.CacheFolderView = new(taskContext, LauncherRuntime.LocalCacheDirectory);
        Storage.DataFolderView = new(taskContext, LauncherRuntime.DataDirectory);
        Storage.InstallFolderView = new(taskContext, PackageIdentityAdapter.AppDirectory);

        UpdateInfo = updateService.UpdateInfo;

        try
        {
            bool startup = autoStartService.IsStartupEnabled();
            bool runElevated = autoStartService.IsRunElevatedEnabled();
            AppOptions options = appOptionsInstance;
            appOptions = options;

            options.RunElevated.PropertyChanged += (s, e) =>
            {
                try
                {
                    taskContext.InvokeOnMainThread(() => OnPropertyChanged(nameof(RunElevated)));
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }
            };

            options.IsStartupEnabled.PropertyChanged += (s, e) =>
            {
                try
                {
                    taskContext.InvokeOnMainThread(() => OnPropertyChanged(nameof(IsStartupEnabled)));
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }
            };

            OnPropertyChanged(nameof(RunElevated));
            OnPropertyChanged(nameof(IsStartupEnabled));
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb($"Setting load failed: {ex.Message}", category: "Setting", level: BreadcrumbLevel.Warning);
        }

        return true;
    }

    [Command("SubmitFeedbackCommand")]
    private async Task SubmitFeedbackAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Submit feedback", "SettingViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory
            .CreateForSelectionAsync(SH.ViewModelSettingFeedbackTitle, SH.ViewModelSettingFeedbackDescription, "GitHub Issues", SH.ViewModelSettingFeedbackDeveloperServer)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            Uri uri = BackendApiRoutes.GitHubIssues.ToUri();
            await OpenGitHubUriAsync(uri).ConfigureAwait(false);
        }
        else if (result is ContentDialogResult.Secondary)
        {
            SubmitFeedbackDialog dialog = await contentDialogFactory.CreateInstanceAsync<SubmitFeedbackDialog>(serviceProvider).ConfigureAwait(false);
            await dialog.ShowFeedbackAsync().ConfigureAwait(false);
        }
    }

    [Command("OpenTranslateCommand")]
    private async Task OpenTranslateAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open translate", "SettingViewModel.Command"));
        await OpenGitHubUriAsync(BackendApiRoutes.GitHubPulls.ToUri()).ConfigureAwait(false);
    }

    [Command("OpenGitHubCommand")]
    private async Task OpenGitHubAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open GitHub", "SettingViewModel.Command"));
        await OpenGitHubUriAsync(BackendApiRoutes.GitHubBase.ToUri()).ConfigureAwait(false);
    }

    private async Task OpenGitHubUriAsync(Uri uri)
    {
        if (HttpProxyUsingSystemProxy.Instance.CurrentProxyUri is null)
        {
            _ = Windows.System.Launcher.LaunchUriAsync(uri);
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        if (currentXamlWindowReference.XamlRoot is { } xamlRoot)
        {
            UrlWebView2ContentProvider provider = new(uri);
            ShowWebView2WindowAction.Show(provider, xamlRoot);
        }
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

        if (result.Kind is not CheckUpdateResultKind.UpdateAvailable || result.PackageInformation is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(result.PackageInformation.PatchUrl) &&
            !string.IsNullOrEmpty(result.PackageInformation.PatchSha256))
        {
            await DownloadPatchAndRestartAsync(result.PackageInformation).ConfigureAwait(false);
        }
        else
        {
            await taskContext.InvokeOnMainThreadAsync(() => UpdateInfo = SH.ViewModelMainUpdatePatchNotAvailable).ConfigureAwait(false);
        }
    }

    private async Task DownloadPatchAndRestartAsync(LauncherPackageInformation packageInfo)
    {
        ITaskContext ctx = serviceProvider.GetRequiredService<ITaskContext>();

        await ctx.InvokeOnMainThreadAsync(() => UpdateInfo = SH.ViewModelSettingPatchDownloading).ConfigureAwait(false);

        bool success = await MainViewModel.DownloadAndExtractPatchAsync(packageInfo).ConfigureAwait(false);

        if (!success)
        {
            await ctx.InvokeOnMainThreadAsync(() => UpdateInfo = SH.ViewModelSettingPatchVerifyFailed).ConfigureAwait(false);
            return;
        }

        await ctx.InvokeOnMainThreadAsync(() => UpdateInfo = SH.ViewModelSettingPatchReadyRestarting).ConfigureAwait(false);
        await Task.Delay(500).ConfigureAwait(false);

        string appDir = PackageIdentityAdapter.AppDirectory;
        string updaterPath = Path.Combine(appDir, "updater.exe");

        if (!File.Exists(updaterPath))
        {
            await ctx.InvokeOnMainThreadAsync(() => UpdateInfo = SH.ViewModelSettingPatchUpdaterMissing).ConfigureAwait(false);
            return;
        }

        string filesDir = Path.Combine(LauncherRuntime.DataDirectory, "UpdateCache", "files");
        int pid = Environment.ProcessId;
        string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName ?? "launcher.exe");

        ctx.DispatcherQueue.TryEnqueue(() =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = updaterPath,
                Arguments = $"--pid {pid} --source \"{filesDir}\" --target \"{appDir}\" --exe \"{exeName}\"",
                UseShellExecute = true,
            });
            Application.Current.Exit();
        });
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
