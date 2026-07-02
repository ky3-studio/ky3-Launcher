//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Launcher.UI.Windowing;
using Launcher.UI.Windowing.Abstraction;
using Launcher.ViewModel.Guide;
using System.Collections.Immutable;
using Windows.Graphics;

namespace Launcher.UI.Xaml.View.Window;

[Service(ServiceLifetime.Transient)]
internal sealed partial class GuideWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    public GuideWindow(IServiceProvider serviceProvider)
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return;
        }

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            SizeInt32 minSize = ScaledSizeInt32.CreateForWindow(1050, 650, this);
            presenter.PreferredMinimumWidth = minSize.Width;
            presenter.PreferredMinimumHeight = minSize.Height;
            SizeInt32 maxSize = ScaledSizeInt32.CreateForWindow(1200, 800, this);
            presenter.PreferredMaximumWidth = maxSize.Width;
            presenter.PreferredMaximumHeight = maxSize.Height;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        GuideView.InitializeDataContext<GuideViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DraggableGrid; }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get => []; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1050, 650, this); }
}
