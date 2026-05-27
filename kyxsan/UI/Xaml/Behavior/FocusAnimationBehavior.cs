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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT;

namespace kyxsan.UI.Xaml.Behavior;

[DependencyProperty<object>("Watch", PropertyChangedCallbackName = nameof(OnWatchChanged))]
[DependencyProperty<Storyboard>("Storyboard")]
internal sealed partial class FocusAnimationBehavior : BehaviorBase<Border>
{
    private static void OnWatchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _)
    {
        FocusAnimationBehavior behavior = sender.As<FocusAnimationBehavior>();
        if (behavior.AssociatedObject is not { } control)
        {
            return;
        }

        if (behavior.Storyboard is { } storyboard)
        {
            storyboard.Stop();
            Storyboard.SetTarget(behavior.Storyboard, control);
            storyboard.Begin();
        }
    }
}