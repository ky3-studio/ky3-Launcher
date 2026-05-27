//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.Property;
using kyxsan.Model.Entity;
using kyxsan.Service.Game;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.Data;

namespace kyxsan.ViewModel.Game;

[BindableCustomPropertyProvider]
internal sealed partial class LaunchSchemeFilteredGameAccountsView : ObservableObject
{
    private readonly AsyncLock syncRoot = new();

    private readonly IGameService gameService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private LaunchScheme? scheme;

    public LaunchSchemeFilteredGameAccountsView(IProperty<bool> isViewUnloaded, IGameService gameService, ITaskContext taskContext, IMessenger messenger)
    {
        IsViewUnloaded = isViewUnloaded;
        this.gameService = gameService;
        this.taskContext = taskContext;
        this.messenger = messenger;
    }

    public LaunchScheme? Scheme
    {
        get => scheme;
        set => SetAsync(value, false).SafeForget();
    }

    [ObservableProperty]
    public partial IAdvancedCollectionView<GameAccount>? View { get; private set; }

    private IProperty<bool> IsViewUnloaded { get; }

    public GameAccount? SelectedGameAccount
    {
        get => View?.CurrentItem;
        set
        {
            if (View is null)
            {
                return;
            }

            View.MoveCurrentTo(value);
            OnPropertyChanged();
        }
    }

    public async ValueTask SetAsync(LaunchScheme? value, bool external = true)
    {
        using (await syncRoot.LockAsync().ConfigureAwait(false))
        {
            await taskContext.SwitchToMainThreadAsync();
            if (!SetProperty(ref scheme, value, nameof(Scheme)))
            {
                return;
            }

            if (View is null)
            {
                IAdvancedCollectionView<GameAccount> accountsView = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(true);
                await taskContext.SwitchToMainThreadAsync();
                View = accountsView;
                OnPropertyChanged(nameof(SelectedGameAccount));
            }
            else
            {
                // Clear the selected game account to prevent setting
                // incorrect CN/OS registry account when scheme not match
                await taskContext.SwitchToMainThreadAsync();
                View.MoveCurrentTo(default);
                OnPropertyChanged(nameof(SelectedGameAccount));
            }

            await taskContext.SwitchToMainThreadAsync();
            View.Filter = GameAccountFilter.Create(Scheme?.SchemeType);
            OnPropertyChanged(nameof(SelectedGameAccount));

            // Try set to the current registry account.
            if (Scheme is null)
            {
                if (external)
                {
                    messenger.Send(InfoBarMessage.Warning(SH.ViewModelLaunchGameSchemeNotSelected));
                }

                return;
            }

            if (View is null)
            {
                return;
            }

            // The GameAccount is guaranteed to be in the view, because the scheme is synced
            // Except when scheme is bilibili, which is not supported
            if (View.CurrentItem is null)
            {
                View.MoveCurrentTo(gameService.DetectCurrentGameAccountNoThrow(Scheme));
                OnPropertyChanged(nameof(SelectedGameAccount));
            }
        }
    }
}