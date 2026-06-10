//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;

namespace kyxsan.UI.Xaml.Control.Card;

[DependencyProperty<Brush>("ProgressForeground")]
[DependencyProperty<Brush>("TextForeground")]
[DependencyProperty<double>("Maximum", NotNull = true)]
[DependencyProperty<double>("Value", NotNull = true)]
[DependencyProperty<bool>("IsIndeterminate", NotNull = true, DefaultValue = false)]
[DependencyProperty<string>("Header")]
[DependencyProperty<string>("Description")]
internal sealed partial class CardProgressBar : Microsoft.UI.Xaml.Controls.Control
{
    public CardProgressBar()
    {
        DefaultStyleKey = typeof(CardProgressBar);
    }
}