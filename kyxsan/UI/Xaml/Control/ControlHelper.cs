//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace kyxsan.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<Brush>("Background", PropertyChangedCallbackName = nameof(OnBackgroundChanged), IsAttached = true, TargetType = typeof(Microsoft.UI.Xaml.Controls.Control))]
public sealed partial class ControlHelper
{
    private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not Microsoft.UI.Xaml.Controls.Control control)
        {
            return;
        }

        control.Background = (Brush)args.NewValue;
    }
}