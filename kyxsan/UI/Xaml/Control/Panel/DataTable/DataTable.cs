//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace kyxsan.UI.Xaml.Control.Panel.DataTable;

[DependencyProperty<double>("ColumnSpacing", NotNull = true)]
internal sealed partial class DataTable : Microsoft.UI.Xaml.Controls.Panel
{
    internal HashSet<DataRow> Rows { get; } = [];

    internal void ColumnResized()
    {
        InvalidateMeasure();

        foreach (DataRow row in Rows)
        {
            row.InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double fixedWidth = 0;
        double proportionalUnits = 0;
        double autoSized = 0;
        double maxWidth = 0;

        double maxHeight = 0;

        List<DataColumn> elements = Children.Where(static e => e.Visibility == Visibility.Visible).OfType<DataColumn>().ToList();

        // We only need to measure elements that are visible
        foreach (DataColumn column in elements)
        {
            if (column.CurrentWidth.IsStar)
            {
                proportionalUnits += column.DesiredWidth.Value;
            }
            else if (column.CurrentWidth.IsAbsolute)
            {
                fixedWidth += column.DesiredWidth.Value;
            }
        }

        // Add in spacing between columns to our fixed size allotment
        fixedWidth += (elements.Count - 1) * ColumnSpacing;

        double proportionalAmount = (availableSize.Width - fixedWidth) / proportionalUnits;

        foreach (DataColumn column in elements)
        {
            if (column.CurrentWidth.IsStar)
            {
                column.Measure(new Size(proportionalAmount * column.CurrentWidth.Value, availableSize.Height));
            }
            else if (column.CurrentWidth.IsAbsolute)
            {
                column.Measure(new Size(column.CurrentWidth.Value, availableSize.Height));
            }
            else
            {
                column.Measure(new Size(Math.Max(0, availableSize.Width - fixedWidth - autoSized), availableSize.Height));

                // Keep track of already 'allotted' space, use either the maximum child size (if we know it) or the header content
                autoSized += Math.Max(column.DesiredSize.Width, column.MaxChildDesiredWidth);
            }

            maxWidth += column.MaxChildDesiredWidth;
            maxHeight = Math.Max(maxHeight, column.DesiredSize.Height);
        }

        maxWidth += (elements.Count - 1) * ColumnSpacing;
        return new(Math.Min(availableSize.Width, maxWidth), maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double fixedWidth = 0;
        double proportionalUnits = 0;
        double autoSized = 0;

        List<DataColumn> elements = Children.Where(static e => e.Visibility == Visibility.Visible).OfType<DataColumn>().ToList();

        // We only need to measure elements that are visible
        foreach (DataColumn column in elements)
        {
            if (column.CurrentWidth.IsStar)
            {
                proportionalUnits += column.CurrentWidth.Value;
            }
            else if (column.CurrentWidth.IsAbsolute)
            {
                fixedWidth += column.CurrentWidth.Value;
            }
            else
            {
                autoSized += Math.Max(column.DesiredSize.Width, column.MaxChildDesiredWidth);
            }
        }

        double proportionalAmount = (finalSize.Width - fixedWidth - autoSized) / proportionalUnits;

        double width = 0;
        double x = 0;

        foreach (DataColumn column in elements)
        {
            if (column.CurrentWidth.IsStar)
            {
                width = proportionalAmount * column.CurrentWidth.Value;
            }
            else if (column.CurrentWidth.IsAbsolute)
            {
                width = column.CurrentWidth.Value;
            }
            else
            {
                width = column.MaxChildDesiredWidth;
            }

            if (width is 0)
            {
                continue;
            }

            column.Arrange(new(x, 0, width, finalSize.Height));
            x += width + ColumnSpacing;
        }

        return new(Math.Max(0, x - ColumnSpacing), finalSize.Height);
    }
}