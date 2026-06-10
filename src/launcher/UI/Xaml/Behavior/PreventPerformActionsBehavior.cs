//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace kyxsan.UI.Xaml.Behavior;

internal sealed class PreventPerformActionsBehavior : BehaviorBase<UIElement>
{
    protected override bool Initialize()
    {
        AssociatedObject.PointerPressed += OnPointerEvent;
        AssociatedObject.PointerReleased += OnPointerEvent;
        AssociatedObject.RightTapped += OnRightTapped;
        return true;
    }

    protected override bool Uninitialize()
    {
        AssociatedObject.PointerPressed -= OnPointerEvent;
        AssociatedObject.PointerReleased -= OnPointerEvent;
        AssociatedObject.RightTapped -= OnRightTapped;
        return true;
    }

    private static void OnPointerEvent(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private static void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        e.Handled = true;
    }
}