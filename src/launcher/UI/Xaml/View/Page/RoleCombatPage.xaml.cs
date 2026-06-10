//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using kyxsan.Service.User;
using kyxsan.UI.Content;
using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.RoleCombat;
using kyxsan.ViewModel.User;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class RoleCombatPage : ScopedPage, IRecipient<UserAndUidChangedMessage>
{
    private IServiceProvider? serviceProvider;
    private IMessenger? messenger;
    private UserViewModel? userViewModel;
    private bool viewModelInitialized;

    public RoleCombatPage()
    {
        InitializeComponent();
        MainPivot.Items.Remove(StatisticsPivotItem);
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is not null)
        {
            DispatcherQueue.TryEnqueue(() => ShowLoggedInContent());
        }
    }

    protected override async void LoadingOverride()
    {
        if (XamlRoot.XamlContext() is not { } context)
        {
            return;
        }

        serviceProvider = context.ServiceProvider;
        messenger = serviceProvider.GetRequiredService<IMessenger>();
        messenger.Register<UserAndUidChangedMessage>(this);

        Unloaded += OnPageUnloaded;

        IUserService userService = serviceProvider.GetRequiredService<IUserService>();

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(true) is null)
        {
            ShowNotLoggedIn();
        }
        else
        {
            ShowLoggedInContent();
        }
    }

    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        messenger?.UnregisterAll(this);
        Unloaded -= OnPageUnloaded;
    }

    private async void ShowLoggedInContent()
    {
        if (serviceProvider is null)
        {
            return;
        }

        NotLoggedInPanel.Visibility = Visibility.Collapsed;
        LoggedInPanel.Visibility = Visibility.Visible;

        if (!viewModelInitialized)
        {
            viewModelInitialized = true;
            InitializeDataContext<RoleCombatViewModel>();
        }

        if (DataContext is RoleCombatViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(default).ConfigureAwait(false);
        }
    }

    private void ShowNotLoggedIn()
    {
        if (serviceProvider is null)
        {
            return;
        }

        userViewModel = serviceProvider.GetRequiredService<UserViewModel>();

        LoginByQRCodeCard.Command = userViewModel.LoginByQRCodeCommand;
        LoginByMobileCaptchaCard.Command = userViewModel.LoginByMobileCaptchaCommand;
        ManualInputCard.Command = userViewModel.AddUserCommand;

        NotLoggedInPanel.Visibility = Visibility.Visible;
        LoggedInPanel.Visibility = Visibility.Collapsed;
    }
}
