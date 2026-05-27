//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using kyxsan.Core.Logging;
using kyxsan.UI.Windowing;
using kyxsan.UI.Windowing.Abstraction;
using kyxsan.Web.WebView2;
using kyxsan.Win32.Foundation;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.UI.Xaml.View.Window.WebView2;

[SuppressMessage("", "CA1001")]
internal sealed partial class WebView2Window : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowClosedHandler
{
    private readonly CancellationTokenSource loadCts = new();
    private readonly SemaphoreSlim webview2LoadLock = new(1, 1);
    private readonly IServiceScope scope;

    private readonly IWebView2ContentProvider contentProvider;
    private readonly WindowId parentWindowId;

    public WebView2Window(IServiceProvider serviceProvider, WindowId parentWindowId, IWebView2ContentProvider contentProvider)
    {
        this.parentWindowId = parentWindowId;

        try
        {
            WindowUtilities.SetWindowOwner(this.GetWindowHandle(), Win32Interop.GetWindowFromWindowId(parentWindowId));
        }
        catch (FileLoadException)
        {
        }

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsModal = true;
            presenter.IsResizable = false;
            presenter.IsMinimizable = false;
            presenter.IsMaximizable = false;
        }

        this.contentProvider = contentProvider;
        contentProvider.CloseWindowAction = Close;

        InitializeComponent();

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;

        scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleArea; }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get => []; }

    public new void Activate()
    {
        HWND parentHwnd = Win32Interop.GetWindowFromWindowId(parentWindowId);
        WindowUtilities.SwitchToWindow(parentHwnd);
        WindowUtilities.SetWindowIsEnabled(parentHwnd, false);
        base.Activate();

        AppWindow.MoveThenResize(contentProvider.InitializePosition(AppWindow.GetFromWindowId(parentWindowId).Rect, this.RasterizationScale));
    }

    public void OnWindowClosing(out bool cancel)
    {
        cancel = false;
        loadCts.Cancel();
    }

    public void OnWindowClosed()
    {
        HWND parentHwnd = Win32Interop.GetWindowFromWindowId(parentWindowId);
        WindowUtilities.SetWindowIsEnabled(parentHwnd, true);
        WindowUtilities.SwitchToWindow(parentHwnd);

        if (webview2LoadLock.Wait(TimeSpan.Zero))
        {
            webview2LoadLock.Release();
        }

        webview2LoadLock.Dispose();
    }

    [Command("GoBackCommand")]
    private void GoBack()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Go back", "WebView2Window.Command"));

        if (WebView?.CoreWebView2 is null)
        {
            return;
        }

        if (WebView.CoreWebView2.CanGoBack)
        {
            WebView.CoreWebView2.GoBack();
        }
    }

    [Command("RefreshCommand")]
    private void Refresh()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh", "WebView2Window.Command"));

        if (WebView?.CoreWebView2 is null)
        {
            return;
        }

        try
        {
            WebView.CoreWebView2.Reload();
        }
        catch (COMException)
        {
        }
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        OnWebViewLoadedAsync().SafeForget();

        [SuppressMessage("", "SH003")]
        async Task OnWebViewLoadedAsync()
        {
            await webview2LoadLock.WaitAsync().ConfigureAwait(true);

            try
            {
                try
                {
                    CoreWebView2Environment environment = await CoreWebView2EnvironmentFactory.GetAsync().ConfigureAwait(true);
                    await WebView.EnsureCoreWebView2Async(environment);
                }
                catch (Exception ex) when (ex is SEHException or OperationCanceledException or COMException)
                {
                    LoadingRing.Visibility = Visibility.Collapsed;
                    WebView2LoadFailedHintText.Visibility = Visibility.Visible;
                    return;
                }

                if (loadCts.IsCancellationRequested || WebView?.CoreWebView2 is null)
                {
                    LoadingRing.Visibility = Visibility.Collapsed;
                    WebView2LoadFailedHintText.Visibility = Visibility.Visible;
                    return;
                }

                LoadingRing.Visibility = Visibility.Collapsed;
                WebView.CoreWebView2.DocumentTitleChanged += OnDocumentTitleChanged;
                WebView.CoreWebView2.HistoryChanged += OnHistoryChanged;
                WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
                contentProvider.CoreWebView2 = WebView.CoreWebView2;
                await contentProvider.InitializeAsync(scope.ServiceProvider, loadCts.Token).ConfigureAwait(false);
            }
            finally
            {
                webview2LoadLock.Release();
            }
        }
    }

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        loadCts.Cancel();
        loadCts.Dispose();
        contentProvider.Unload();

        if (WebView.CoreWebView2 is not null)
        {
            WebView.CoreWebView2.DocumentTitleChanged -= OnDocumentTitleChanged;
            WebView.CoreWebView2.HistoryChanged -= OnHistoryChanged;
        }

        WebView.Loaded -= OnWebViewLoaded;
        WebView.Unloaded -= OnWebViewUnloaded;
    }

    private void OnDocumentTitleChanged(CoreWebView2 sender, object args)
    {
        DocumentTitle.Text = sender.DocumentTitle;
    }

    private void OnHistoryChanged(CoreWebView2 sender, object args)
    {
        GoBackButton.IsEnabled = sender.CanGoBack;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        contentProvider.ActualTheme = sender.ActualTheme;
    }
}
