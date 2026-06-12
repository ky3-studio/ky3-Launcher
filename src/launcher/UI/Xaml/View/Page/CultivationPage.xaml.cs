// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using kyxsan.UI.Xaml.Control;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using kyxsan.ViewModel.Cultivation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class CultivationPage : ScopedPage
{
    public CultivationPage()
    {
        InitializeComponent();
        IsTabStop = true;
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<CultivationViewModel>();
    }

    private void OnRootGridPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement source &&
            source.FindAscendant<AutoSuggestTokenBox>() is null)
        {
            Focus(FocusState.Programmatic);
        }
    }
}
