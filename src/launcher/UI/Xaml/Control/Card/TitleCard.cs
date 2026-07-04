//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace Launcher.UI.Xaml.Control.Card;

[ContentProperty(Name = nameof(Content))]
internal sealed partial class TitleCard : Microsoft.UI.Xaml.Controls.Control
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(TitleCard), new PropertyMetadata(default(string), OnVisualPropertyChanged));

    public static readonly DependencyProperty TitleContentProperty =
        DependencyProperty.Register(nameof(TitleContent), typeof(object), typeof(TitleCard), new PropertyMetadata(default(object), OnVisualPropertyChanged));

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(TitleCard), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty DividerVisibilityProperty =
        DependencyProperty.Register(nameof(DividerVisibility), typeof(Visibility), typeof(TitleCard), new PropertyMetadata(Visibility.Visible));

    public TitleCard()
    {
        DefaultStyleKey = typeof(TitleCard);
    }

    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public object? TitleContent
    {
        get => (object?)GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }

    public object? Content
    {
        get => (object?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public Visibility DividerVisibility
    {
        get => (Visibility)GetValue(DividerVisibilityProperty);
        set => SetValue(DividerVisibilityProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SetVisualStates();
    }

    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TitleCard card)
        {
            card.SetVisualStates();
        }
    }

    private void SetVisualStates()
    {
        if (string.IsNullOrEmpty(Title) && TitleContent is null)
        {
            VisualStateManager.GoToState(this, "TitleGridCollapsed", true);
            DividerVisibility = Visibility.Collapsed;
        }
        else
        {
            VisualStateManager.GoToState(this, "TitleGridVisible", true);
            DividerVisibility = Visibility.Visible;
        }
    }
}