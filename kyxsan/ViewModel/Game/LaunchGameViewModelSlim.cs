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

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.Logging;
using kyxsan.Core.Property;
using kyxsan.Factory.ContentDialog;
using kyxsan.Model.Entity;
using kyxsan.Service.Game;
using kyxsan.Service.Game.Package;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.User;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.UI.Xaml.View.Page;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using System.Diagnostics;
using BindingUser = kyxsan.ViewModel.User.User;

namespace kyxsan.ViewModel.Game;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<LaunchGamePage>,
    IViewModelSupportLaunchExecution,
    IRecipient<UserAndUidChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial LaunchGameViewModelSlim(IServiceProvider serviceProvider);

    public partial LaunchGameShared Shared { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public LaunchSchemeFilteredGameAccountsView CurrentSchemeFilteredGameAccountsView { get => field ??= new(Property.Create(false), gameService, taskContext, messenger); }

    [ObservableProperty]
    public partial UserGameRole? CurrentUserGameRole { get; set; }

    LaunchScheme? IViewModelSupportLaunchExecution.TargetScheme { get => CurrentSchemeFilteredGameAccountsView.Scheme; }

    LaunchScheme? IViewModelSupportLaunchExecution.CurrentScheme { get => CurrentSchemeFilteredGameAccountsView.Scheme; }

    GameAccount? IViewModelSupportLaunchExecution.GameAccount { get => CurrentSchemeFilteredGameAccountsView.View?.CurrentItem; }

    ValueTask<BlockDeferral<PackageConvertStatus>> IViewModelSupportLaunchExecution.CreateConvertBlockDeferralAsync()
    {
        // Should never happen: slim does not support package conversion.
        Debugger.Break();
        return BlockDeferral<PackageConvertStatus>.CreateAsync<LaunchGamePackageConvertDialog>(serviceProvider, static (state, dialog) => dialog.State = state);
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        // There is no way to change UsingHoyolabAccount when viewing cards.
        // So we don't need to update CurrentUserGameRole when UsingHoyolabAccount changed.
        if (!LaunchOptions.UsingHoyolabAccount.Value)
        {
            return;
        }

        taskContext.InvokeOnMainThread(() =>
        {
            // We will fetch the UserGameRole when loading.
            if (!IsInitialized)
            {
                return;
            }

            CurrentUserGameRole = message.User?.UserGameRoles.CurrentItem;
        });
    }

    protected override async Task LoadAsync()
    {
        if (Shared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } scheme)
        {
            return;
        }

        await CurrentSchemeFilteredGameAccountsView.SetAsync(scheme).ConfigureAwait(true);
        Shared.ResumeLaunchExecutionAsync(this).SafeForget();

        UserGameRole? userGameRole = LaunchOptions.UsingHoyolabAccount.Value
            ? await userService.GetCurrentUserGameRoleAsync().ConfigureAwait(false)
            : default;

        await taskContext.SwitchToMainThreadAsync();
        CurrentUserGameRole = userGameRole;
        IsInitialized = true;
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch game", "LaunchGameViewModelSlim.Command"));

        UserAndUid? userAndUid;
        if (LaunchOptions.UsingHoyolabAccount.Value)
        {
            // 优先使用独立保存的米游社账号，与左栏无关
            string savedMid = LaunchOptions.SelectedHoyolabUserMid.Value;
            BindingUser? selectedUser = string.IsNullOrEmpty(savedMid)
                ? null
                : await userService.GetUserByMidAsync(savedMid).ConfigureAwait(false);
            UserAndUid.TryFromUser(selectedUser, out userAndUid);
            if (userAndUid is null)
            {
                userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
            }
        }
        else
        {
            userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        }

        if (LaunchOptions.AdvancedStartDelayedOnGameLaunch.Value)
        {
            Shared.LaunchAdvancedDelayedAsync().SafeForget();
        }
        await Shared.DefaultLaunchExecutionAsync(this, userAndUid).ConfigureAwait(false);
    }
}