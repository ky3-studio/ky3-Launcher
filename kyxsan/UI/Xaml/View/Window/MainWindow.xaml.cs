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
using kyxsan.Service;
using kyxsan.UI.Shell;
using kyxsan.UI.Windowing;
using Microsoft.Windows.AppNotifications.Builder;
using kyxsan.UI.Windowing.Abstraction;
using kyxsan.ViewModel;
using System.Collections.Immutable;
using Windows.Graphics;

namespace kyxsan.UI.Xaml.View.Window;

[Service(ServiceLifetime.Transient)]
internal sealed partial class MainWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowClosedHandler,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    private readonly App app;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            SizeInt32 minSize = ScaledSizeInt32.CreateForWindow(900, 735, this);
            presenter.PreferredMinimumWidth = minSize.Width;
            presenter.PreferredMinimumHeight = minSize.Height;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);

        MainView.InitializeDataContext<MainViewModel>(scope.ServiceProvider);

        app = scope.ServiceProvider.GetRequiredService<App>();
    }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1351, 761, this); }

    public FrameworkElement TitleBarCaptionAccess { get => MainView.TitleBar; }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get => []; }

    public void OnWindowClosing(out bool cancel)
    {
        cancel = false;
        AppWindow.Hide();
    }

    public void OnWindowClosed()
    {
        if (XamlApplicationLifetime.Exiting)
        {
            return;
        }

        if (!XamlApplicationLifetime.NotifyIconCreated || app.Options.LastWindowCloseBehavior.Value is LastWindowCloseBehavior.ExitApplication)
        {
            app.Exit();
            return;
        }

        if (this.TryGetAssociatedServiceProvider(out IServiceProvider serviceProvider) && !serviceProvider.GetRequiredService<NotifyIconController>().IsPromoted())
        {
            try
            {
                new AppNotificationBuilder()
                    .AddText(SH.CoreWindowingNotifyIconPromotedHint)
                    .Show();
            }
            catch
            {
                // Ignore
            }
        }
    }
}