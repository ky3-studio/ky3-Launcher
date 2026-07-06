//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.ExceptionService;
using Launcher.Core.Property;
using Launcher.Core.Setting;
using Launcher.Factory.ContentDialog;
using Launcher.Factory.Picker;
using Launcher.Model;
using Launcher.Model.Entity;
using Launcher.Model.Intrinsic;
using Launcher.Service.Game;
using Launcher.Service.Game.AdvancedStart;
using Launcher.Service.Game.Locator;
using Launcher.Service.Game.Package;
using Launcher.Service.Game.PathAbstraction;
using Launcher.Service.Game.Scheme;
using Launcher.Service.Navigation;
using Launcher.Service.Notification;
using Launcher.Core.Database;
using Launcher.Service.User;
using Launcher.UI.Input.HotKey;
using Launcher.UI.Input.LowLevel;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.Web.ThirdPartyTool;
using System.Collections.Immutable;
using BindingUser = Launcher.ViewModel.User.User;
using EntityUser = Launcher.Model.Entity.User;

namespace Launcher.ViewModel.Game;

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
                LaunchOptions.SelectedHoyolabUserMid.Value = value?.Entity?.Mid ?? string.Empty;
            }
        }
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
            LaunchScheme? currentScheme;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                currentScheme = GamePathEntry.Value is not null
                    ? Shared.GetCurrentLaunchSchemeFromConfigurationFile()
                    : default;
            }

            await taskContext.SwitchToMainThreadAsync();
            await TargetSchemeFilteredGameAccountsView.SetAsync(currentScheme).ConfigureAwait(true);
            await GamePackageViewModel.ReloadAsync().ConfigureAwait(true);
        }
        catch (LauncherException ex)
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
}
