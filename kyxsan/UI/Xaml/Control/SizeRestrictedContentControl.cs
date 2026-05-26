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
using Windows.Foundation;

namespace kyxsan.UI.Xaml.Control;

[DependencyProperty<bool>("IsWidthRestricted", DefaultValue = true, NotNull = true)]
[DependencyProperty<bool>("IsHeightRestricted", DefaultValue = true, NotNull = true)]
internal sealed partial class SizeRestrictedContentControl : ContentControl
{
    private double minContentWidth;
    private double minContentHeight;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Content is FrameworkElement element)
        {
            element.Measure(availableSize);
            Size contentDesiredSize = element.DesiredSize;

            Size contentActualOrDesiredSize = new(
                Math.Clamp(element.ActualWidth, contentDesiredSize.Width, availableSize.Width),
                Math.Clamp(element.ActualHeight, contentDesiredSize.Height, availableSize.Height));

            if (minContentWidth > availableSize.Width)
            {
                minContentWidth = 0;
            }

            if (minContentHeight > availableSize.Height)
            {
                minContentHeight = 0;
            }

            if (IsWidthRestricted)
            {
                if (contentActualOrDesiredSize.Width > minContentWidth)
                {
                    minContentWidth = contentActualOrDesiredSize.Width;
                }

                element.MinWidth = minContentWidth;
            }

            if (IsHeightRestricted)
            {
                if (contentActualOrDesiredSize.Height > minContentHeight)
                {
                    minContentHeight = contentActualOrDesiredSize.Height;
                }

                element.MinHeight = minContentHeight;
            }
        }

        return base.MeasureOverride(availableSize);
    }
}