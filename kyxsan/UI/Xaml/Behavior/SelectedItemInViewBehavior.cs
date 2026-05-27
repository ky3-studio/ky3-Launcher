//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;

namespace kyxsan.UI.Xaml.Behavior;

internal sealed class SelectedItemInViewBehavior : BehaviorBase<ListViewBase>
{
    protected override bool Initialize()
    {
        if (AssociatedObject.SelectedItem is { } item)
        {
            AssociatedObject.SmoothScrollIntoViewWithItemAsync(item, ScrollItemPlacement.Center).SafeForget();
        }

        return true;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        if (AssociatedObject.SelectedItem is { } item)
        {
            AssociatedObject.SmoothScrollIntoViewWithItemAsync(item, ScrollItemPlacement.Center).SafeForget();
        }
    }
}