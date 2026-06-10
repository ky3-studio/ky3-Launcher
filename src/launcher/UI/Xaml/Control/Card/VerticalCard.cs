//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace kyxsan.UI.Xaml.Control.Card;

[DependencyProperty<UIElement>("Top")]
[DependencyProperty<UIElement>("Bottom")]
[DependencyProperty<Thickness>("BottomPadding", NotNull = true)]
[DependencyProperty<HorizontalAlignment>("HorizontalTopAlignment", DefaultValue = HorizontalAlignment.Stretch, NotNull = true)]
[DependencyProperty<VerticalAlignment>("VerticalTopAlignment", DefaultValue = VerticalAlignment.Stretch, NotNull = true)]
[DependencyProperty<HorizontalAlignment>("HorizontalBottomAlignment", DefaultValue = HorizontalAlignment.Center, NotNull = true)]
[DependencyProperty<VerticalAlignment>("VerticalBottomAlignment", DefaultValue = VerticalAlignment.Stretch, NotNull = true)]
internal sealed partial class VerticalCard : Microsoft.UI.Xaml.Controls.Control
{
    public VerticalCard()
    {
        DefaultStyleKey = typeof(VerticalCard);
    }
}