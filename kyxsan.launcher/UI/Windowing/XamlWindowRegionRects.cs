//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using kyxsan.Core.Graphics;
using kyxsan.UI.Windowing.Abstraction;
using Windows.Foundation;
using Windows.Graphics;

namespace kyxsan.UI.Windowing;

internal static class XamlWindowRegionRects
{
    public static void Update(Window window)
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        // E_UNEXPECTED will be thrown if the Content is not loaded.
        if (!xamlWindow.TitleBarCaptionAccess.IsLoaded)
        {
            return;
        }

        InputNonClientPointerSource inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        {
            FrameworkElement element = xamlWindow.TitleBarCaptionAccess;
            Point position = element.TransformToVisual(window.Content).TransformPoint(default);
            RectInt32 rect = RectInt32Convert.RectInt32(position, element.ActualSize).Scale(window.RasterizationScale);
            inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption, [rect]);
        }

        List<RectInt32> passthrough = [];
        foreach (FrameworkElement element in xamlWindow.TitleBarPassthrough)
        {
            if (element.Visibility is not Visibility.Visible)
            {
                continue;
            }

            Point position = element.TransformToVisual(window.Content).TransformPoint(default);
            RectInt32 rect = RectInt32Convert.RectInt32(position, element.ActualSize).Scale(window.RasterizationScale);

            if (rect.Size > 0)
            {
                passthrough.Add(rect);
            }
        }

        if (passthrough.Count > 0)
        {
            inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Passthrough, [.. passthrough]);
        }
    }
}