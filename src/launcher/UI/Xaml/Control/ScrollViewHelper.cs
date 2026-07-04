//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Launcher.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<UIElement>("RightPanel", IsAttached = true, TargetType = typeof(ScrollView))]
[DependencyProperty<object>(
    "ScrollToTopAssociatedObject",
    PropertyChangedCallbackName = nameof(OnScrollToTopAssociatedObjectChanged),
    IsAttached = true,
    TargetType = typeof(ScrollView))]
public sealed partial class ScrollViewHelper
{
    private static void OnScrollToTopAssociatedObjectChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        if (dp is not ScrollView { IsLoaded: true } scrollView)
        {
            return;
        }

        if (args.OldValue != args.NewValue)
        {
            scrollView.ScrollPresenter.ScrollTo(0, 0);
        }
    }
}