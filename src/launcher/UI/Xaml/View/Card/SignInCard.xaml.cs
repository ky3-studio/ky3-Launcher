//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Launcher.ViewModel.Abstraction;
using Launcher.ViewModel.Sign;
using Launcher.ViewModel.User;

namespace Launcher.UI.Xaml.View.Card;

internal sealed partial class SignInCard : Button
{
    public SignInCard(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.InitializeViewModelSlim<SignInViewModel>(serviceProvider);
        this.DataContext<SignInViewModel>()?.AttachXamlElement(AwardScrollViewer);
    }

    public SignInCard(IServiceProvider serviceProvider, UserAndUid userAndUid)
    {
        InitializeComponent();
        this.InitializeViewModelSlim<SignInViewModel>(serviceProvider);

        SignInViewModel? viewModel = this.DataContext<SignInViewModel>();
        if (viewModel is not null)
        {
            viewModel.TargetUserAndUid = userAndUid;
            viewModel.AttachXamlElement(AwardScrollViewer);
        }
    }

    private void CheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.DataContext<SignInViewModel>()?.IsAutoCheckIn = true;
    }

    private void CheckBox_Unchecked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.DataContext<SignInViewModel>()?.IsAutoCheckIn = false;
    }
}
