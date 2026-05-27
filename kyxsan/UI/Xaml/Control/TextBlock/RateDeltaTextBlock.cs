//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace kyxsan.UI.Xaml.Control.TextBlock;

[TemplateVisualState(Name = "PositiveValue", GroupName = "CommonStates")]
[TemplateVisualState(Name = "NegativeValue", GroupName = "CommonStates")]
[DependencyProperty<string>("Text", PropertyChangedCallbackName = nameof(OnTextPropertyChanged))]
[DependencyProperty<Style>("TextStyle")]
internal sealed partial class RateDeltaTextBlock : ContentControl
{
    public RateDeltaTextBlock()
    {
        DefaultStyleKey = typeof(RateDeltaTextBlock);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateState();
    }

    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        RateDeltaTextBlock control = d.As<RateDeltaTextBlock>();
        control.UpdateState();
    }

    private void UpdateState()
    {
        if (Text is { Length: > 0 } text)
        {
            _ = text.AsSpan()[0] switch
            {
                '+' => VisualStateManager.GoToState(this, "PositiveValue", true),
                '-' => VisualStateManager.GoToState(this, "NegativeValue", true),
                _ => VisualStateManager.GoToState(this, "NoValue", true),
            };
        }
        else
        {
            VisualStateManager.GoToState(this, "NoValue", true);
        }
    }
}