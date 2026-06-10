//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using kyxsan.Core.Graphics;
using kyxsan.UI.Windowing;
using kyxsan.UI.Windowing.Abstraction;
using kyxsan.UI.Xaml.Media.Backdrop;
using kyxsan.Win32.Foundation;
using Windows.Foundation;

namespace kyxsan.UI.Shell;

internal sealed class NotifyIconXamlHostWindow : Window, IWindowNeedEraseBackground, IXamlWindowClosedHandler
{
    public NotifyIconXamlHostWindow(IServiceProvider serviceProvider)
    {
        Content = new Border();

        this.AddExtendedStyleLayered();
        this.SetLayeredWindowTransparency(0);
        this.AddExtendedStyleToolWindow();

        AppWindow.Title = "SnapkyxsanNotifyIconXamlHost";
        AppWindow.SafeIsShowInSwitchers(false);

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
    }

    public void ShowFlyoutAt(FlyoutBase flyout, Point point, RECT icon)
    {
        icon.left -= 8;
        icon.top -= 8;
        icon.right += 8;
        icon.bottom += 8;

        if (AppWindow is null || Content?.XamlRoot is null /*ERROR_XAMLROOT_REQUIRED*/)
        {
            return;
        }

        if (flyout.IsDisposed)
        {
            return;
        }

        this.SwitchTo();
        MoveAndResize(icon);

        flyout.ShowAt(Content, new()
        {
            Placement = FlyoutPlacementMode.Auto,
            ShowMode = FlyoutShowMode.Standard,
        });
    }

    public void MoveAndResize(RECT icon)
    {
        AppWindow.MoveAndResize(RectInt32Convert.RectInt32(icon));
    }

    public void OnWindowClosing(out bool cancel)
    {
        // https://github.com/DGP-Studio/kyxsan/issues/2532
        // Prevent the window closing when the application is not exiting.
        cancel = !XamlApplicationLifetime.Exiting;
    }

    public void OnWindowClosed()
    {
    }
}