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

namespace kyxsan.UI.Xaml.Control.Panel.DataTable;

[DependencyProperty<bool>("CanResize", NotNull = true)]
[DependencyProperty<GridLength>("DesiredWidth", PropertyChangedCallbackName = nameof(OnDesiredWidthPropertyChanged), CreateDefaultValueCallbackName = nameof(CreateDesiredWidthDefaultValue), NotNull = true)]
internal sealed partial class DataColumn : ContentControl
{
    public DataColumn()
    {
        DefaultStyleKey = typeof(DataColumn);
    }

    internal double MaxChildDesiredWidth { get; set; }

    internal GridLength CurrentWidth { get; private set; }

    private static object CreateDesiredWidthDefaultValue()
    {
        return GridLength.Auto;
    }

    private static void OnDesiredWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // If the developer updates the size of the column, update our internal copy
        if (d is DataColumn column)
        {
            column.CurrentWidth = column.DesiredWidth;
        }
    }
}