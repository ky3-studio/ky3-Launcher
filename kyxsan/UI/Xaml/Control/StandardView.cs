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

namespace kyxsan.UI.Xaml.Control;

[TemplateVisualState(Name = "Show", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Empty", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Hide", GroupName = "CommonStates")]
[DependencyProperty<UIElement>("EmptyContent")]
[DependencyProperty<bool>("ShowCondition", DefaultValue = false, PropertyChangedCallbackName = nameof(OnShowConditionChanged), NotNull = true)]
[DependencyProperty<bool>("HideCondition", DefaultValue = false, PropertyChangedCallbackName = nameof(OnHideConditionChanged), NotNull = true)]
internal sealed partial class StandardView : ContentControl
{
    public StandardView()
    {
        DefaultStyleKey = typeof(StandardView);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateState();
    }

    private static void OnShowConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StandardView view)
        {
            view.UpdateState();
        }
    }

    private static void OnHideConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StandardView view)
        {
            view.UpdateState();
        }
    }

    private void UpdateState()
    {
        if (HideCondition)
        {
            VisualStateManager.GoToState(this, "Hide", true);
            return;
        }

        if (ShowCondition)
        {
            VisualStateManager.GoToState(this, "Show", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "Empty", true);
        }
    }
}