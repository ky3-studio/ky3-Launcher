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

using Microsoft.UI.Xaml.Controls;
using kyxsan.UI.Xaml;
using kyxsan.ViewModel;

namespace kyxsan.UI.Shell;

internal sealed partial class NotifyIconContextMenu : Flyout
{
    public NotifyIconContextMenu(IServiceProvider serviceProvider)
    {
        AllowFocusOnInteraction = false;
        InitializeComponent();
        Root.InitializeDataContext<NotifyIconViewModel>(serviceProvider);

        if (Root.DataContext is NotifyIconViewModel viewModel)
        {
            viewModel.XamlRoot = Root.XamlRoot;
            viewModel.SetNotifyIconContextMenu(this, Root);
        }

        Closed += OnClosed;
    }

    private void OnClosed(object? sender, object args)
    {
        if (Root.DataContext is NotifyIconViewModel viewModel)
        {
            viewModel.NotifyIconContextMenuClosed();
        }
    }
}