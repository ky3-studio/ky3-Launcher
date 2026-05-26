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
using kyxsan.Core.Graphics;
using kyxsan.Core.Logging;
using kyxsan.Service.Game.Package.Advanced;
using kyxsan.UI.Windowing;
using kyxsan.UI.Windowing.Abstraction;
using kyxsan.ViewModel.Game;
using System.Collections.Immutable;
using Windows.Graphics;

namespace kyxsan.UI.Xaml.View.Window;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class GamePackageOperationWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowClosedHandler
{
    private readonly TaskCompletionSource closeTcs = new();

    public GamePackageOperationWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        RectInt32 workArea = DisplayArea.Primary.WorkArea;
        SizeInt32 size = new(workArea.Height, (int)(workArea.Height * 0.75));
        AppWindow.Resize(size.Scale(0.5 * this.RasterizationScale));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        RootGrid.InitializeDataContext<GamePackageOperationViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DraggableGrid; }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get => []; }

    public Task CloseTask { get => closeTcs.Task; }

    public void OnWindowClosing(out bool cancel)
    {
        cancel = RootGrid.DataContext<GamePackageOperationViewModel>() is { IsFinished: false };
    }

    public void OnWindowClosed()
    {
        closeTcs.TrySetResult();
    }

    internal void HandleProgressUpdate(GamePackageOperationReport status)
    {
        RootGrid.DataContext<GamePackageOperationViewModel>()?.HandleProgressUpdate(status);
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Close Window", "GamePackageOperationWindow.Command"));
        Close();
    }
}