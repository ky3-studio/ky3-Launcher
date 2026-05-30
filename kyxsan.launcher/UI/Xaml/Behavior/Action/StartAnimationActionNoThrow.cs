//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace kyxsan.UI.Xaml.Behavior.Action;

[DependencyProperty<AnimationSet>("Animation")]
[DependencyProperty<UIElement>("TargetObject")]
internal sealed partial class StartAnimationActionNoThrow : DependencyObject, IAction
{
    /// <inheritdoc/>
    public object Execute(object sender, object parameter)
    {
        if (Animation is not null)
        {
            if (TargetObject is not null)
            {
                Animation.Start(TargetObject);
            }
            else
            {
                Animation.Start(sender as UIElement);
            }
        }

        return default!;
    }
}