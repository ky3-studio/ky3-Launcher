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
using kyxsan.ViewModel;
using kyxsan.ViewModel.User;

namespace kyxsan.UI.Xaml.View;

internal sealed partial class MainView : UserControl, IDataContextInitialized
{
    public MainView()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
    }

    public void OnDataContextInitialized(IServiceProvider serviceProvider)
    {
        UserView.InitializeDataContext<UserViewModel>(serviceProvider);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= OnUnloaded;
        this.DataContext<MainViewModel>()?.Uninitialize();
    }
}