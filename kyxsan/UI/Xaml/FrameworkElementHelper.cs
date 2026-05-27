//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace kyxsan.UI.Xaml;

[SuppressMessage("", "SH001")]
[DependencyProperty<double>("SquareLength", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnSquareLengthChanged), IsAttached = true, TargetType = typeof(FrameworkElement), NotNull = true)]
public sealed partial class FrameworkElementHelper
{
    private static void OnSquareLengthChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)dp;
        element.Width = (double)e.NewValue;
        element.Height = (double)e.NewValue;
    }
}