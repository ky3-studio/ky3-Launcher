//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.User;
using kyxsan.UI.Content;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class UserPage : ScopedPage
{
    public UserPage()
    {
        InitializeComponent();
    }

    protected override async void LoadingOverride()
    {
        if (XamlRoot.XamlContext() is not { } context)
        {
            return;
        }

        UserViewModel userViewModel = context.ServiceProvider.GetRequiredService<UserViewModel>();
        DataContext = userViewModel;
        
        await userViewModel.LoadCommand.ExecuteAsync(null).ConfigureAwait(true);
    }
}
