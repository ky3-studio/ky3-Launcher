//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.Shell;
using System.Runtime.CompilerServices;
using WinRT.Interop;

namespace kyxsan.UI.Windowing;

internal static class WindowExtension
{
    private static readonly ConditionalWeakTable<Window, XamlWindowController> WindowControllers = [];

    extension<TWindow>(TWindow window)
        where TWindow : Window
    {
        public void InitializeController(IServiceProvider serviceProvider)
        {
            XamlWindowController windowController = new(window, serviceProvider);
            WindowControllers.Add(window, windowController);
        }

        public void UninitializeController()
        {
            WindowControllers.Remove(window);
        }

        public bool TrySetTaskbarProgress(TBPFLAG state, ulong value, ulong maximum)
        {
            if (!WindowControllers.TryGetValue(window, out XamlWindowController? controller))
            {
                return false;
            }

            return controller.TrySetTaskbarProgress(state, value, maximum);
        }
    }

    extension(Window window)
    {
        public double RasterizationScale
        {
            get
            {
                return window is { Content.XamlRoot: { } xamlRoot }
                    ? xamlRoot.RasterizationScale
                    : WindowUtilities.GetRasterizationScaleForWindow(window.GetWindowHandle());
            }
        }

        public bool TryGetAssociatedServiceProvider(out IServiceProvider serviceProvider)
        {
            if (WindowControllers.TryGetValue(window, out XamlWindowController? controller))
            {
                serviceProvider = controller.ServiceProvider;
                return true;
            }

            serviceProvider = default!;
            return false;
        }

        public HWND GetWindowHandle()
        {
            return WindowNative.GetWindowHandle(window);
        }

        public void SwitchTo()
        {
            WindowUtilities.SwitchToWindow(window.GetWindowHandle());
        }

        public void AddExtendedStyleLayered()
        {
            WindowUtilities.AddExtendedStyleLayered(window.GetWindowHandle());
        }

        public void RemoveExtendedStyleLayered()
        {
            WindowUtilities.RemoveExtendedStyleLayered(window.GetWindowHandle());
        }

        public void SetLayeredWindowTransparency(byte alpha)
        {
            WindowUtilities.SetLayeredWindowTransparency(window.GetWindowHandle(), alpha);
        }

        public void AddExtendedStyleToolWindow()
        {
            WindowUtilities.AddExtendedStyleToolWindow(window.GetWindowHandle());
        }

        public void RemoveStyleOverlappedWindow()
        {
            WindowUtilities.RemoveStyleOverlappedWindow(window.GetWindowHandle());
        }
    }
}