//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Service.Navigation.Message;
using kyxsan.UI.Content;

namespace kyxsan.UI.Xaml.Behavior;

internal sealed class ServiceRecipientTitleBarBehavior : BehaviorBase<TitleBar>
{
    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.BackRequested += OnBackRequested;
        AssociatedObject.PaneToggleRequested += OnPaneToggleRequested;
    }

    protected override bool Uninitialize()
    {
        AssociatedObject.BackRequested -= OnBackRequested;
        AssociatedObject.PaneToggleRequested -= OnPaneToggleRequested;
        return base.Uninitialize();
    }

    private static void OnBackRequested(TitleBar sender, object args)
    {
        sender.XamlRoot.XamlContext()?.ServiceProvider.GetRequiredService<IMessenger>().Send(new NavigationGoBackMessage());
    }

    private static void OnPaneToggleRequested(TitleBar sender, object args)
    {
        sender.XamlRoot.XamlContext()?.ServiceProvider.GetRequiredService<IMessenger>().Send(new NavigationPaneToggleMessage());
    }
}