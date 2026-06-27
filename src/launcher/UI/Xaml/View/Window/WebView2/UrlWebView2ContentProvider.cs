using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Windows.Graphics;

namespace Launcher.UI.Xaml.View.Window.WebView2;

internal sealed class UrlWebView2ContentProvider : IWebView2ContentProvider
{
    private readonly Uri targetUri;

    public UrlWebView2ContentProvider(Uri uri)
    {
        targetUri = uri;
    }

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    public ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);
        CoreWebView2.NewWindowRequested += OnNewWindowRequested;
        CoreWebView2.Navigate(targetUri.OriginalString);
        return ValueTask.CompletedTask;
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        return WebView2WindowPosition.Padding(parentRect, 48);
    }

    public void Unload()
    {
        if (CoreWebView2 is not null)
        {
            CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
        }
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        _ = Windows.System.Launcher.LaunchUriAsync(e.Uri.ToUri());
    }
}
