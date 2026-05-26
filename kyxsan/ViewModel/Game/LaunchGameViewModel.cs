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

using kyxsan.Core.ExceptionService;
using kyxsan.Core.IO;
using kyxsan.Core.Logging;
using kyxsan.Core.Property;
using kyxsan.Core.Setting;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Factory.Process;
using kyxsan.Model;
using kyxsan.Model.Entity;
using kyxsan.Model.Intrinsic;
using kyxsan.Service.Game;
using kyxsan.Service.Game.AdvancedStart;
using kyxsan.Service.Game.AdvancedStart.Model;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Locator;
using kyxsan.Service.Game.Package;
using kyxsan.Service.Game.PathAbstraction;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.Navigation;
using kyxsan.Service.Notification;
using kyxsan.Service.ThirdPartyTool;
using kyxsan.Core.Database;
using kyxsan.Service.User;
using kyxsan.UI.Input.HotKey;
using kyxsan.UI.Input.LowLevel;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.UI.Xaml.View.Window;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using kyxsan.Web.ThirdPartyTool;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Windows.System;
using BindingUser = kyxsan.ViewModel.User.User;
using EntityUser = kyxsan.Model.Entity.User;

namespace kyxsan.ViewModel.Game;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchGameViewModel : Abstraction.ViewModel, IViewModelSupportLaunchExecution, INavigationRecipient
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly AdvancedStartDelayedProgramStore store;

    [GeneratedConstructor]
    public partial LaunchGameViewModel(IServiceProvider serviceProvider);

    public partial GamePackageViewModel GamePackageViewModel { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial LowLevelKeyOptions LowLevelKeyOptions { get; }

    public partial HotKeyOptions HotKeyOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public partial LaunchGameShared Shared { get; }

    public ImmutableArray<LaunchScheme> KnownSchemes { get; } = KnownLaunchSchemes.Values;

    private IObservableProperty<ImmutableArray<ToolInfo>> thirdPartyToolsField = new ObservableProperty<ImmutableArray<ToolInfo>>(ImmutableArray<ToolInfo>.Empty);
    public IObservableProperty<ImmutableArray<ToolInfo>> ThirdPartyTools { get => thirdPartyToolsField; }

    public string AdvancedStartProgramPath
    {
        get => field;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    LaunchScheme? IViewModelSupportLaunchExecution.TargetScheme { get => TargetSchemeFilteredGameAccountsView.Scheme; }

    LaunchScheme? IViewModelSupportLaunchExecution.CurrentScheme { get => Shared.GetCurrentLaunchSchemeFromConfigurationFile(); }

    GameAccount? IViewModelSupportLaunchExecution.GameAccount { get => TargetSchemeFilteredGameAccountsView.View?.CurrentItem; }

    public LaunchSchemeFilteredGameAccountsView TargetSchemeFilteredGameAccountsView { get => field ??= new(IsViewUnloaded, gameService, taskContext, messenger); private set; }

    public IObservableProperty<NameValue<PlatformType>?> SelectedPlatformType { get => field ??= LaunchOptions.PlatformType.AsNameValue(LaunchOptions.PlatformTypes); }

    public IObservableProperty<GamePathEntry?> GamePathEntry { get => field ??= LaunchOptions.GamePathEntry.SetWithCondition(static (value, unloaded) => !unloaded.Value && value is not null, IsViewUnloaded); }

    public IReadOnlyObservableProperty<string> DisplayGamePath { get => field ??= Property.Observe(LaunchOptions.GamePathEntry, static entry => SH.FormatViewModelLaunchGameDisplayGamePath(entry?.Path)); }

    public IReadOnlyObservableProperty<bool> GamePathEntryValid { get => field ??= Property.Observe(LaunchOptions.GamePathEntry, static entry => !string.IsNullOrEmpty(entry?.Path)).WithValueChangedCallback(static (v, vm) => vm.HandleGamePathEntryChangeAsync().SafeForget(), this); }

    public IReadOnlyObservableProperty<bool> IsIslandConnected { get => GameLifeCycle.IsIslandConnected.AsReadOnly(); }

    public AdvancedDbCollectionView<BindingUser, EntityUser>? Users { get; private set => SetProperty(ref field, value); }

    public BindingUser? SelectedLaunchUser
    {
        get => field;
        set
        {
            if (SetProperty(ref field, value))
            {
                // 持久化选择，使其完全独立于左栏账号
                LaunchOptions.SelectedHoyolabUserMid.Value = value?.Entity?.Mid ?? string.Empty;
            }
        }
    }

    public ObservableCollection<AdvancedStartDelayedProgramEntry> Entries { get; private set => SetProperty(ref field, value); } = [];

    private AdvancedStartDelayedProgramEntry? selectedDelayedProgramEntry;

    public AdvancedStartDelayedProgramEntry? SelectedDelayedProgramEntry
    {
        get => selectedDelayedProgramEntry;
        set => SetProperty(ref selectedDelayedProgramEntry, value);
    }

    public async ValueTask<bool> ReceiveAsync(INavigationExtraData data, CancellationToken token)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (data is LaunchGameExtraData { TypedData: { } uid })
        {
            return await userService.SetCurrentUserByUidAsync(uid).ConfigureAwait(false);
        }

        return false;
    }

    [SuppressMessage("", "SH003")]
    public async Task HandleGamePathEntryChangeAsync()
    {
        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                LaunchScheme? currentScheme = GamePathEntry.Value is not null
                    ? Shared.GetCurrentLaunchSchemeFromConfigurationFile()
                    : default;

                await taskContext.SwitchToMainThreadAsync();
                await TargetSchemeFilteredGameAccountsView.SetAsync(currentScheme).ConfigureAwait(true);
                await GamePackageViewModel.ReloadAsync().ConfigureAwait(true);
            }
        }
        catch (kyxsanException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    ValueTask<BlockDeferral<PackageConvertStatus>> IViewModelSupportLaunchExecution.CreateConvertBlockDeferralAsync()
    {
        return BlockDeferral<PackageConvertStatus>.CreateAsync<LaunchGamePackageConvertDialog>(serviceProvider, static (state, dialog) => dialog.State = state);
    }

    private readonly object delayedSaveGate = new();
    private CancellationTokenSource? delayedSaveCts;
    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        AdvancedStartProgramPath = LocalSetting.Get(SettingKeys.LaunchAdvancedStartProgramPath, string.Empty);

        try
        {
            Entries = store.Load();
        }
        catch
        {
            Entries = [];
        }

        WireEntries(Entries);

        if (LaunchOptions.GamePathEntries.Value.IsDefaultOrEmpty)
        {
            await serviceProvider.GetRequiredService<IGamePathService>().SilentLocateAllGamePathAsync().ConfigureAwait(false);
        }

        await HandleGamePathEntryChangeAsync().ConfigureAwait(false);
        Shared.ResumeLaunchExecutionAsync(this).SafeForget();

        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);

        // 恢复上次保存的独立账号选择（与左栏无关）
        string savedMid = LaunchOptions.SelectedHoyolabUserMid.Value;
        BindingUser? savedUser = string.IsNullOrEmpty(savedMid)
            ? null
            : await userService.GetUserByMidAsync(savedMid).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        Users = users;
        SelectedLaunchUser = savedUser ?? users.CurrentItem;

        InitializeThirdPartyToolsInBackgroundAsync(token).SafeForget();

        return true;
    }

    private async Task InitializeThirdPartyToolsInBackgroundAsync(CancellationToken token)
    {
        try
        {
            await Task.Yield();

            if (token.IsCancellationRequested || IsViewUnloaded.Value)
            {
                return;
            }

            ImmutableArray<ToolInfo> tools = await InitializeThirdPartyToolsAsync(token).ConfigureAwait(false);

            if (token.IsCancellationRequested || IsViewUnloaded.Value)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            if (!token.IsCancellationRequested && !IsViewUnloaded.Value)
            {
                thirdPartyToolsField.Value = tools;
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb($"Failed to initialize third party tools: {ex.Message}", category: "ThirdPartyTool");
            SentrySdk.CaptureException(ex);
        }
    }

    private async ValueTask<ImmutableArray<ToolInfo>> InitializeThirdPartyToolsAsync(CancellationToken token)
    {
        try
        {
            SentrySdk.AddBreadcrumb("Starting to initialize third party tools", category: "ThirdPartyTool");
            IThirdPartyToolService thirdPartyToolService = serviceProvider.GetRequiredService<IThirdPartyToolService>();
            SentrySdk.AddBreadcrumb("Got IThirdPartyToolService instance", category: "ThirdPartyTool");

            token.ThrowIfCancellationRequested();
            ImmutableArray<ToolInfo> tools = await thirdPartyToolService.GetToolsAsync().ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            SentrySdk.AddBreadcrumb($"Got {tools.Length} tools from service", category: "ThirdPartyTool");
            return tools;
        }
        catch (OperationCanceledException)
        {
            return ImmutableArray<ToolInfo>.Empty;
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb($"Failed to initialize third party tools: {ex.Message}", category: "ThirdPartyTool");
            SentrySdk.CaptureException(ex);
            return ImmutableArray<ToolInfo>.Empty;
        }
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

    [Command("PickGamePathCommand")]
    private async Task PickGamePathAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set game path by picker", "LaunchGameViewModel.Command"));
        if (await gameLocatorFactory.LocateSingleAsync(GameLocationSourceKind.Manual).ConfigureAwait(false) is not (true, var path))
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.PerformGamePathEntrySynchronization(path);
    }

    [Command("ResetGamePathCommand")]
    private void ResetGamePath()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset game path", "LaunchGameViewModel.Command"));
        LaunchOptions.GamePathEntry.Value = default;
        _ = 1;
    }

    [Command("RemoveGamePathEntryCommand")]
    private void RemoveGamePathEntry(GamePathEntry? entry)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove game path", "LaunchGameViewModel.Command"));
        LaunchOptions.RemoveGamePathEntry(entry);
    }

    [Command("RemoveAspectRatioCommand")]
    private void RemoveAspectRatio(AspectRatio? aspectRatio)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove aspect ratio", "LaunchGameViewModel.Command"));
        if (aspectRatio is null)
        {
            return;
        }

        if (aspectRatio.Equals(LaunchOptions.SelectedAspectRatio))
        {
            LaunchOptions.SelectedAspectRatio = default;
        }

        LaunchOptions.AspectRatios.Remove(aspectRatio);
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch game", "LaunchGameViewModel.Command"));

        if (LaunchOptions.AdvancedStartDelayedOnGameLaunch.Value)
        {
            Shared.LaunchAdvancedDelayedAsync().SafeForget();
        }

        UserAndUid? userAndUid;
        if (LaunchOptions.UsingHoyolabAccount.Value && SelectedLaunchUser is not null)
        {
            // 非左栏当前用户的 UserGameRoles.CurrentItem 可能未初始化，
            // 需要先恢复角色选择，否则 TryFromUser 会因 CurrentItem 为 null 而失败
            if (SelectedLaunchUser.UserGameRoles.CurrentItem is null)
            {
                bool restored = false;
                string? preferredUid = SelectedLaunchUser.PreferredUid;
                if (!string.IsNullOrEmpty(preferredUid))
                {
                    foreach (UserGameRole role in SelectedLaunchUser.UserGameRoles)
                    {
                        if (role.GameUid == preferredUid)
                        {
                            SelectedLaunchUser.UserGameRoles.MoveCurrentTo(role);
                            restored = true;
                            break;
                        }
                    }
                }

                if (!restored)
                {
                    SelectedLaunchUser.UserGameRoles.MoveCurrentToFirst();
                }
            }

            UserAndUid.TryFromUser(SelectedLaunchUser, out userAndUid);
        }
        else
        {
            userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        }

        await Shared.DefaultLaunchExecutionAsync(this, userAndUid).ConfigureAwait(false);
    }

    [Command("ConvertCommand")]
    private async Task ConvertAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Convert game server", "LaunchGameViewModel.Command"));
        LaunchScheme? previousScheme = Shared.GetCurrentLaunchSchemeFromConfigurationFile(false);
        bool success = await Shared.ConvertLaunchExecutionAsync(this).ConfigureAwait(false);
        await HandleGamePathEntryChangeAsync().ConfigureAwait(false);

        if (success)
        {
            LaunchScheme? newScheme = Shared.GetCurrentLaunchSchemeFromConfigurationFile(false);
            if (newScheme is not null && !ReferenceEquals(newScheme, previousScheme))
            {
                string serverName = newScheme.DisplayName;
                string desc = newScheme.Description;
                string message = string.IsNullOrEmpty(desc)
                    ? string.Format(SH.ViewModelServerConvertSuccessText, serverName)
                    : string.Format(SH.ViewModelServerConvertSuccessWithDescText, serverName, desc);
                messenger.Send(InfoBarMessage.Success(message));
            }
        }
    }

    [Command("CleanConvertResourceCommand")]
    private async Task CleanConvertResourceAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Clean convert resource", "LaunchGameViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            LaunchGameCleanResourceDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<LaunchGameCleanResourceDialog>(scope.ServiceProvider)
                .ConfigureAwait(false);

            ImmutableArray<CleanResourceItem> selectedItems = await dialog.GetSelectedItemsAsync().ConfigureAwait(false);

            if (selectedItems.IsDefaultOrEmpty)
            {
                return;
            }

            long totalCleaned = 0;
            int successCount = 0;
            List<string> errors = [];

            foreach (CleanResourceItem item in selectedItems)
            {
                try
                {
                    if (Directory.Exists(item.Path))
                    {
                        Directory.Delete(item.Path, true);
                        totalCleaned += item.Size;
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{item.Name}: {ex.Message}");
                }
            }

            if (successCount > 0)
            {
                string sizeText = LaunchGameCleanResourceDialog.FormatSize(totalCleaned);
                messenger.Send(InfoBarMessage.Success(string.Format(SH.ViewModelCleanResourceSuccessText, successCount, sizeText)));
            }

            if (errors.Count > 0)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelCleanResourcePartialError, string.Join("\n", errors)));
            }
        }
    }

    [Command("DetectGameAccountCommand")]
    private async Task DetectGameAccountAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Detect registry game account", "LaunchGameViewModel.Command"));

        try
        {
            if (TargetSchemeFilteredGameAccountsView.Scheme is null)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameSchemeNotSelected));
                return;
            }

            if (TargetSchemeFilteredGameAccountsView.View is null)
            {
                return;
            }

            GameAccount? currentAccount = await gameService.DetectGameAccountAsync(TargetSchemeFilteredGameAccountsView.Scheme, async (suggestedName) =>
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                        .GetRequiredService<IContentDialogFactory>()
                        .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider, suggestedName)
                        .ConfigureAwait(false);
                    return await dialog.GetInputNameAsync().ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            if (currentAccount is not null)
            {
                await taskContext.SwitchToMainThreadAsync();
                TargetSchemeFilteredGameAccountsView.View.MoveCurrentTo(currentAccount);
            }
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("ModifyGameAccountCommand")]
    private async Task ModifyGameAccountAsync(GameAccount? gameAccount)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Modify registry game account", "LaunchGameViewModel.Command"));

        if (gameAccount is null)
        {
            return;
        }

        await gameService.ModifyGameAccountAsync(gameAccount, async originalName =>
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                    .GetRequiredService<IContentDialogFactory>()
                    .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider, originalName)
                    .ConfigureAwait(false);

                return await dialog.GetInputNameAsync().ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    [Command("RemoveGameAccountCommand")]
    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove registry game account", "LaunchGameViewModel.Command"));

        if (gameAccount is null)
        {
            return;
        }

        await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
    }

    [Command("OpenScreenshotFolderCommand")]
    private async Task OpenScreenshotFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open screenshot folder", "LaunchGameViewModel.Command"));

        const string LockTrace = $"{nameof(LaunchGameViewModel)}.{nameof(OpenScreenshotFolderAsync)}";
        if (LaunchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);
        using (gameFileSystem)
        {
            Directory.CreateDirectory(gameFileSystem.ScreenShotDirectory);
            await Windows.System.Launcher.LaunchFolderPathAsync(gameFileSystem.ScreenShotDirectory);
        }
    }

    [Command("KillGameProcessCommand")]
    private async Task KillGameProcess()
    {
        if (!LaunchOptions.CanKillGameProcess.Value)
        {
            return;
        }

        await GameLifeCycle.TryKillGameProcessAsync(taskContext).ConfigureAwait(false);
    }

    [Command("ShowThirdPartyToolDialogCommand")]
    private async Task ShowThirdPartyToolDialogAsync(ToolInfo? tool)
    {
        if (tool is null)
        {
            return;
        }

        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Show third party tool dialog", "LaunchGameViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ThirdPartyToolDialog dialog = await scope.ServiceProvider
                .GetRequiredService<IContentDialogFactory>()
                .CreateInstanceAsync<ThirdPartyToolDialog>(scope.ServiceProvider, tool);

            await contentDialogFactory.EnqueueAndShowAsync(dialog).ShowTask;
        }
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

        await Launcher.LaunchUriAsync("about:blank".ToUri());
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

    [Command("PickBgiPathCommand")]
    private async Task PickBgiPathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.BgiPath.Value = file;
    }

    [Command("PickAttachProgramCommand")]
    private async Task PickAttachProgramAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.AttachProgramPath.Value = file;
    }

    [Command("PickAttachProgram2Command")]
    private async Task PickAttachProgram2Async()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.AttachProgram2Path.Value = file;
    }

    [Command("PickAttachProgram3Command")]
    private async Task PickAttachProgram3Async()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.AttachProgram3Path.Value = file;
    }

    // Custom DLL Injection Commands
    [Command("AddCustomDllCommand")]
    private async Task AddCustomDllAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "dll", "*.dll");
        if (!ok)
        {
            return;
        }

        string path = file;
        
        await taskContext.SwitchToMainThreadAsync();
        ImmutableDictionary<string, bool> current = LaunchOptions.CustomDllConfigs.Value;
        if (!current.ContainsKey(path))
        {
            LaunchOptions.CustomDllConfigs.Value = current.Add(path, true); // 默认启用
        }
    }

    [Command("RemoveCustomDllCommand")]
    private void RemoveCustomDll(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ImmutableDictionary<string, bool> current = LaunchOptions.CustomDllConfigs.Value;
        LaunchOptions.CustomDllConfigs.Value = current.Remove(path);
    }

    [Command("ToggleCustomDllCommand")]
    private void ToggleCustomDll(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ImmutableDictionary<string, bool> current = LaunchOptions.CustomDllConfigs.Value;
        if (current.TryGetValue(path, out bool enabled))
        {
            LaunchOptions.CustomDllConfigs.Value = current.SetItem(path, !enabled);
        }
    }
}