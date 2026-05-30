//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.LifeCycle.InterProcess;
using kyxsan.Core.Logging;
using kyxsan.Core.Setting;
using kyxsan.Factory.Process;
using kyxsan.Service;
using kyxsan.Service.AutoSignIn;
using kyxsan.Service.BackgroundActivity;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Job;
using kyxsan.Service.Metadata;
using kyxsan.Service.Navigation;
using kyxsan.Service.Notification;
using kyxsan.Service.RemoteConfig;
using kyxsan.Service.Update;
using kyxsan.UI.Input.HotKey;
using kyxsan.UI.Shell;
using kyxsan.UI.Windowing;
using kyxsan.UI.Xaml.View.Page;
using kyxsan.UI.Xaml.View.Window;
using kyxsan.ViewModel.Achievement;
using kyxsan.ViewModel.Game;
using kyxsan.ViewModel.Guide;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.Core.LifeCycle;

[Service(ServiceLifetime.Singleton, typeof(IAppActivation))]
[SuppressMessage("", "CA1001")]
internal sealed partial class AppActivation : IAppActivation, IAppActivationActionHandlersAccess
{
    public const string Action = nameof(Action);
    public const string Uid = nameof(Uid);
    public const string LaunchGame = nameof(LaunchGame);

    private const string CategoryAchievement = "ACHIEVEMENT";
    private const string UrlActionImport = "/IMPORT";

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock activateLock = new();
    private int isActivating;

    [GeneratedConstructor]
    public partial AppActivation(IServiceProvider serviceProvider);

    public void RedirectedActivate(kyxsanActivationArguments args)
    {
        HandleActivationExclusivelyAsync(args).SafeForget();

        async ValueTask HandleActivationExclusivelyAsync(kyxsanActivationArguments args)
        {
            if (Interlocked.CompareExchange(ref isActivating, 1, 0) is not 0)
            {
                return;
            }

            using (await activateLock.LockAsync().ConfigureAwait(false))
            {
                await UnsynchronizedHandleActivationAsync(args).ConfigureAwait(false);
            }

            Interlocked.Exchange(ref isActivating, 0);
        }
    }

    public void NotificationInvoked(AppNotificationManager manager, AppNotificationActivatedEventArgs args)
    {
        HandleAppNotificationActivationAsync(args.Arguments.AsReadOnly(), false).SafeForget();
    }

    public void ActivateAndInitialize(kyxsanActivationArguments args)
    {
        if (Volatile.Read(ref isActivating) is 1)
        {
            return;
        }

        ActivateAndInitializeAsync().SafeForget();

        async ValueTask ActivateAndInitializeAsync()
        {
            try
            {
                using (await activateLock.LockAsync().ConfigureAwait(false))
                {
                    if (Interlocked.CompareExchange(ref isActivating, 1, 0) is not 0)
                    {
                        return;
                    }

                    await UnsynchronizedHandleActivationAsync(args).ConfigureAwait(false);
                    await UnsynchronizedHandleInitializationAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _ = ex;
                throw;
            }
            finally
            {
                XamlApplicationLifetime.ActivationAndInitializationCompleted = true;
                Interlocked.Exchange(ref isActivating, 0);
            }
        }
    }

    public async ValueTask HandleLaunchGameActionAsync(string? uid = null)
    {
        await taskContext.SwitchToMainThreadAsync();

        GuideState guideState = UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language);
        if (guideState < GuideState.Completed)
        {
            if (currentXamlWindowReference.Window is null)
            {
                GuideWindow guideWindow = serviceProvider.GetRequiredService<GuideWindow>();
                currentXamlWindowReference.Window = guideWindow;
                guideWindow.SwitchTo();
                guideWindow.AppWindow.MoveInZOrderAtTop();
            }

            return;
        }

        switch (currentXamlWindowReference.Window)
        {
            case null:
            case MainWindow:
                if (await WaitWindowAsync<MainWindow>().ConfigureAwait(true) is not null)
                {
                    INavigationService navigationService = serviceProvider.GetRequiredService<INavigationService>();
                    await navigationService.NavigateAsync<LaunchGamePage>(LaunchGameExtraData.CreateForUid(uid), true).ConfigureAwait(false);
                }

                return;
        }
    }

    private async ValueTask UnsynchronizedHandleActivationAsync(kyxsanActivationArguments args)
    {
        await taskContext.SwitchToBackgroundAsync();
        switch (args.Kind)
        {
            case kyxsanActivationKind.Protocol:
                {
                    ArgumentNullException.ThrowIfNull(args.ProtocolActivatedUri);
                    await HandleProtocolActivationAsync(args.ProtocolActivatedUri, args.IsRedirectTo).ConfigureAwait(false);
                    break;
                }

            case kyxsanActivationKind.Launch:
                {
                    ArgumentNullException.ThrowIfNull(args.LaunchActivatedArguments);
                    await HandleLaunchActivationAsync(args.IsRedirectTo).ConfigureAwait(false);
                    break;
                }

            case kyxsanActivationKind.AppNotification:
                {
                    ArgumentNullException.ThrowIfNull(args.AppNotificationActivatedArguments);
                    await HandleAppNotificationActivationAsync(args.AppNotificationActivatedArguments, args.IsRedirectTo).ConfigureAwait(false);
                    break;
                }
        }
    }

    private async ValueTask UnsynchronizedHandleInitializationAsync()
    {
        try
        {
            if (kyxsanRuntime.IsProcessElevated)
            {
                serviceProvider.GetRequiredService<AutoStartService>().EnsureUpToDate();
            }
        }
        catch
        {
        }

        serviceProvider.GetRequiredService<PrivateNamedPipeServer>().Start();
        Bootstrap.UseNamedPipeRedirection();

        App app = serviceProvider.GetRequiredService<App>();
        await taskContext.SwitchToMainThreadAsync();
        try
        {
            app.DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
        }
        catch (COMException ex) when (ex.HResult == unchecked((int)0x8001010E))
        {
            ProcessFactory.KillCurrent();
        }

        lock (NotifyIconController.InitializationSyncRoot)
        {
            try
            {
                serviceProvider.GetRequiredService<NotifyIconController>().Create();
                XamlApplicationLifetime.NotifyIconCreated = true;
            }
            catch (Exception ex)
            {
                serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(new kyxsanException(SH.CoreLifeCycleAppActivationNotifyIconCreateFailed, ex)));
            }
        }

        await taskContext.SwitchToBackgroundAsync();

        await Task.WhenAll(
        [
            ConfigureSentryIpAsync(),
            serviceProvider.GetRequiredService<HotKeyOptions>().InitializeAsync().AsTask(),
            serviceProvider.GetRequiredService<kyxsanUserOptions>().InitializeAsync().AsTask(),
            serviceProvider.GetRequiredService<IMetadataService>().InitializeInternalAsync().AsTask(),
            serviceProvider.GetRequiredService<IQuartzService>().StartAsync()
        ]).ConfigureAwait(false);

        BackgroundActivityOptions backgroundActivityOptions = serviceProvider.GetRequiredService<BackgroundActivityOptions>();
        string dllPath = Path.Combine(AppContext.BaseDirectory, "Ky3-luancher-Plugin.dll");
        backgroundActivityOptions.FullTrustInitialization.Update(taskContext, File.Exists(dllPath) ? SH.ServiceBackgroundActivityFullTrustReady : SH.ServiceBackgroundActivityFullTrustNotReady, true, false, false, false);

        Window? mainWindow = await WaitWindowAsync<MainWindow>().ConfigureAwait(false);

        _ = serviceProvider.GetRequiredService<SignInServerDayRolloverScheduler>();
        _ = serviceProvider.GetRequiredService<AutoSignInTriggerService>();

        IMessenger messenger = serviceProvider.GetRequiredService<IMessenger>();
        bool windowIsActive = true;
        bool pendingAnnouncementNotification = false;

        if (mainWindow is not null)
        {
            mainWindow.Activated += (_, e) =>
            {
                bool isActive = e.WindowActivationState != Microsoft.UI.Xaml.WindowActivationState.Deactivated;
                if (isActive && !windowIsActive && pendingAnnouncementNotification)
                {
                    pendingAnnouncementNotification = false;
                    messenger.Send(InfoBarMessage.Information("新公告已发布，请前往公告页面查看阅读"));
                }
                windowIsActive = isActive;
            };
        }

        AppAnnouncementService.Changed += items =>
        {
            if (AppAnnouncementService.ShouldNotify(items))
            {
                if (windowIsActive)
                    messenger.Send(InfoBarMessage.Information("新公告已发布，请前往公告页面查看阅读"));
                else
                    pendingAnnouncementNotification = true;
            }
        };
        AppAnnouncementService.StartPolling();

        if (AppAnnouncementService.ShouldNotify(AppAnnouncementService.Current))
        {
            if (windowIsActive)
                messenger.Send(InfoBarMessage.Information("新公告已发布，请前往公告页面查看阅读"));
            else
                pendingAnnouncementNotification = true;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(3000).ConfigureAwait(false);
                IUpdateService updateService = serviceProvider.GetRequiredService<IUpdateService>();
                CheckUpdateResult result = await updateService.CheckUpdateAsync().ConfigureAwait(false);
                if (result.Kind is CheckUpdateResultKind.UpdateAvailable)
                {
                    await updateService.TriggerUpdateAsync(result).ConfigureAwait(false);
                }
            }
            catch
            {
            }
        });

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2000).ConfigureAwait(false);
                List<FeedbackReply> replies = await FeedbackService.GetUnreadRepliesAsync().ConfigureAwait(false);
                if (replies.Count > 0)
                {
                    string preview = replies[0].Reply;
                    if (preview.Length > 60) preview = preview[..60] + "...";
                    serviceProvider.GetRequiredService<IMessenger>()
                        .Send(InfoBarMessage.Information($"您的反馈已收到回复：{preview}"));
                    await FeedbackService.MarkRepliesReadAsync().ConfigureAwait(false);
                }
            }
            catch
            {
            }
        });

        _ = Task.Run(async () =>
        {
            try
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");
                string lastDate = LocalSetting.Get(SettingKeys.ClientLastHeartbeatDate, "");
                if (lastDate != today)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    await FeedbackService.HeartbeatAsync().ConfigureAwait(false);
                    LocalSetting.Set(SettingKeys.ClientLastHeartbeatDate, today);
                }
            }
            catch
            {
            }
        });

        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Initialization completed", "Application"));
    }

    private async Task ConfigureSentryIpAsync()
    {
        try
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<SentryIpAddressEnricher>().ConfigureAsync().ConfigureAwait(false);
        }
        catch
        {
        }
    }

    private async ValueTask HandleProtocolActivationAsync(Uri uri, bool isRedirectTo)
    {
        UriBuilder builder = new(uri);

        string category = builder.Host.ToUpperInvariant();
        string action = builder.Path.ToUpperInvariant();

        switch (category)
        {
            case CategoryAchievement:
                {
                    await WaitWindowAsync<MainWindow>().ConfigureAwait(false);
                    switch (action)
                    {
                        case UrlActionImport:
                            {
                                await taskContext.SwitchToMainThreadAsync();

                                INavigationCompletionSource navigationAwaiter = new NavigationExtraData(AchievementViewModel.ImportUIAFFromClipboard);
#pragma warning disable CA1849
                                // We can't await there to navigate to Achievement Page, the Achievement
                                // ViewModel requires the Metadata Service to be initialized.
                                // Which is initialized in there (AppActivation - Initialization) which is after Activation.
                                // Thus await would cause a deadlock.
                                // ReSharper disable once MethodHasAsyncOverload
                                serviceProvider
                                    .GetRequiredService<INavigationService>()
                                    .Navigate<AchievementPage>(navigationAwaiter, true);
#pragma warning restore CA1849
                                break;
                            }
                    }

                    break;
                }

            default:
                {
                    await HandleLaunchActivationAsync(isRedirectTo).ConfigureAwait(false);
                    break;
                }
        }
    }

    private async ValueTask HandleLaunchActivationAsync(bool isRedirectTo)
    {
        if (!isRedirectTo)
        {
            LocalSetting.Update(SettingKeys.LaunchTimes, 0, static x => unchecked(x + 1));
            if (Version.Parse(LocalSetting.Update(SettingKeys.LastVersion, "0.0.0.0", $"{kyxsanRuntime.Version}")) < kyxsanRuntime.Version)
            {
                XamlApplicationLifetime.IsFirstRunAfterUpdate = true;
            }
        }

        GuideState guideState = UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language);
        if (guideState < GuideState.Completed)
        {
            await WaitWindowAsync<GuideWindow>().ConfigureAwait(false);
        }
        else
        {
            await WaitWindowAsync<MainWindow>().ConfigureAwait(false);
        }
    }

    private async ValueTask HandleAppNotificationActivationAsync(IReadOnlyDictionary<string, string> arguments, bool isRedirectTo)
    {
        if (arguments.TryGetValue(Action, out string? action))
        {
            if (action is LaunchGame)
            {
                _ = arguments.TryGetValue(Uid, out string? uid);
                await HandleLaunchGameActionAsync(uid).ConfigureAwait(false);
            }
        }
        else
        {
            await HandleLaunchActivationAsync(isRedirectTo).ConfigureAwait(false);
        }
    }

    private async ValueTask<Window?> WaitWindowAsync<TWindow>()
        where TWindow : Window
    {
        await taskContext.SwitchToMainThreadAsync();

        if (currentXamlWindowReference.Window is not { } window)
        {
            try
            {
                window = serviceProvider.GetRequiredService<TWindow>();
            }
            catch (COMException ex)
            {
                _ = ex;
                if (XamlApplicationLifetime.Exiting)
                {
                    return default;
                }

                throw;
            }

            currentXamlWindowReference.Window = window;
        }

        window.SwitchTo();
        window.AppWindow?.MoveInZOrderAtTop();
        return window;
    }
}