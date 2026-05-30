using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.GachaLog;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class WishCountdownPage : ScopedPage
{
    public WishCountdownPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<WishCountdownViewModel>();
    }

    private void OnBannerDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Countdown countdown } && countdown.LatestBanner is { } banner)
        {
            PreviewImage.Source = banner;
            ImagePreviewOverlay.Visibility = Visibility.Visible;
            e.Handled = true;
        }
    }

    private void OnPreviewClose(object sender, TappedRoutedEventArgs e)
    {
        ClosePreview();
        e.Handled = true;
    }

    private void OnPreviewCloseDouble(object sender, DoubleTappedRoutedEventArgs e)
    {
        ClosePreview();
        e.Handled = true;
    }

    private void ClosePreview()
    {
        ImagePreviewOverlay.Visibility = Visibility.Collapsed;
        PreviewImage.Source = null;
    }
}
