//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core.Database;
using kyxsan.Service.User;
using kyxsan.UI.Content;
using kyxsan.UI.Xaml.Control;
using kyxsan.UI.Xaml.View.Card;
using kyxsan.ViewModel.User;
using BindingUser = kyxsan.ViewModel.User.User;
using EntityUser = kyxsan.Model.Entity.User;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class SignInPage : ScopedPage, IRecipient<UserAndUidChangedMessage>
{
    private IServiceProvider? serviceProvider;
    private IMessenger? messenger;
    private UserViewModel? userViewModel;

    public SignInPage()
    {
        InitializeComponent();
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is not null && LoggedInPanel.Visibility == Visibility.Collapsed)
        {
            DispatcherQueue.TryEnqueue(() => ShowSignInCards());
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
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(true);

        if (users.Source.Count == 0)
        {
            ShowNotLoggedIn();
        }
        else
        {
            ShowSignInCards();
        }
    }

    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        messenger?.UnregisterAll(this);
        Unloaded -= OnPageUnloaded;
    }

    private async void ShowSignInCards()
    {
        if (serviceProvider is null)
        {
            return;
        }

        IUserService userService = serviceProvider.GetRequiredService<IUserService>();
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(true);

        SignInCardsGrid.Children.Clear();

        List<UserAndUid> allUserAndUids = [];
        foreach (BindingUser user in users.Source)
        {
            if (user.UserGameRoles.CurrentItem is null)
            {
                user.UserGameRoles.MoveCurrentToFirst();
            }

            if (UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
            {
                allUserAndUids.Add(userAndUid);
            }
        }

        if (allUserAndUids.Count == 0)
        {
            ShowNotLoggedIn();
            return;
        }

        SignInCardsGrid.Columns = allUserAndUids.Count == 1 ? 1 : 2;

        foreach (UserAndUid userAndUid in allUserAndUids)
        {
            Border cardBorder = new()
            {
                Style = (Style)Application.Current.Resources["AcrylicBorderCardStyle"],
                MinHeight = 500,
            };

            SignInCard signInCard = new(serviceProvider, userAndUid)
            {
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            cardBorder.Child = signInCard;
            SignInCardsGrid.Children.Add(cardBorder);
        }

        NotLoggedInPanel.Visibility = Visibility.Collapsed;
        LoggedInPanel.Visibility = Visibility.Visible;
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
