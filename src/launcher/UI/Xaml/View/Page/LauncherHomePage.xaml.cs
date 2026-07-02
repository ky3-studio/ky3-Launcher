using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Composition;
using Launcher.UI.Xaml.Control;
using Launcher.UI.Content;
using Launcher.Core;
using Launcher.Core.Logging;
using Launcher.ViewModel.LauncherHome;
using Launcher.Service;
using Launcher.Service.BackgroundImage;
using System.IO;
using System.Net.Http;

#pragma warning disable CA1826

namespace Launcher.UI.Xaml.View.Page;

[SuppressMessage("", "CA1001")]
internal sealed partial class LauncherHomePage : ScopedPage
{
    private static readonly string BgCacheDir = Path.Combine(LauncherRuntime.LocalCacheDirectory, "BgCache");
    private static readonly List<BackgroundInfo> s_backgroundList = [];
    private static readonly Dictionary<int, byte[]> s_bgImageCache = [];
    private static readonly Dictionary<int, byte[]> s_themeImageCache = [];
    private static BitmapImage? s_cachedFirstBitmap;
    private static bool s_dataInitialized;
    private static bool s_lastShowDynamic = true;
    private static bool s_lastShowStatic = true;

    private MainView? _mainView;
    private readonly List<BannerData> _bannerList = [];
    private readonly List<PostData> _activityList = [];
    private readonly List<PostData> _announceList = [];
    private readonly List<PostData> _infoList = [];
    private int _currentBannerIndex;
    private static int s_currentBgIndex;
    private string _currentTab = "activity";
    private DispatcherTimer? _bannerTimer;
    private DispatcherTimer? _bgTimer;
    private DispatcherTimer? _gameProcessCheckTimer;
    private bool _isLaunchButtonHovered;
    private static bool s_isVideoMode;
    private bool _videoWebView2Ready;
    private Storyboard? _bgSlideStoryboard;
    private Storyboard? _videoTransitionStoryboard;
    private SpriteVisual? _blurVisual;
    private CompositionEffectBrush? _blurBrush;
    private static bool s_isBackgroundInitialized;
    private bool _isPageActive;
    private AppOptions? _appOptions;
    private IHttpClientFactory? _httpClientFactory;
    private static BackgroundImageType s_lastBackgroundType;
    private static string? s_lastCustomPath;
    private bool _suppressStaticImage;

    public LauncherHomePage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<LauncherHomeViewModel>();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainView = FindParent<MainView>(this);
        _isPageActive = true;

        if (_appOptions is null && XamlRoot.XamlContext()?.ServiceProvider is { } sp)
        {
            _appOptions = sp.GetRequiredService<AppOptions>();
            _httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
        }

        BackgroundImageType bgType = _appOptions?.BackgroundImageType.Value ?? BackgroundImageType.LauncherOfficialLauncher;
        string? customPath = _appOptions?.BackgroundImageCustomPath.Value;

        HomeCardPanel.Visibility = _appOptions?.HomePageCardVisible.Value == false ? Visibility.Collapsed : Visibility.Visible;
        BgIndicatorPanel.Visibility = _appOptions?.HomePageIndicatorVisible.Value == false ? Visibility.Collapsed : Visibility.Visible;

        bool typeChanged = s_isBackgroundInitialized && (s_lastBackgroundType != bgType || (bgType == BackgroundImageType.Custom && s_lastCustomPath != customPath));

        bool currentShowDynamic = _appOptions?.BackgroundShowDynamic.Value ?? true;
        bool currentShowStatic = _appOptions?.BackgroundShowStatic.Value ?? true;
        bool filterChanged = bgType == BackgroundImageType.LauncherOfficialLauncher
            && (s_lastShowDynamic != currentShowDynamic || s_lastShowStatic != currentShowStatic);

        if (filterChanged)
        {
            s_dataInitialized = false;
            s_cachedFirstBitmap = null;
            s_backgroundList.Clear();
            s_bgImageCache.Clear();
            s_themeImageCache.Clear();
            s_lastShowDynamic = currentShowDynamic;
            s_lastShowStatic = currentShowStatic;
            typeChanged = true;
        }

        s_lastBackgroundType = bgType;
        s_lastCustomPath = customPath;
        _suppressStaticImage = bgType == BackgroundImageType.LauncherOfficialLauncher
            && currentShowDynamic && !currentShowStatic;

        if (typeChanged)
        {
            ResetBackgroundState();
        }

        if (_mainView != null)
        {
            if (_mainView.FindName("MainBackdropBorder") is Border backdrop)
            {
                backdrop.Opacity = 0;
                backdrop.Visibility = Visibility.Collapsed;
            }

            if (!typeChanged && bgType == BackgroundImageType.LauncherOfficialLauncher && s_cachedFirstBitmap != null && !_suppressStaticImage && !s_isVideoMode)
            {
                _mainView.LauncherBackgroundImage.Source = s_cachedFirstBitmap;
                _mainView.LauncherBackgroundImage.Opacity = 1;
            }
        }

        if (bgType == BackgroundImageType.None)
        {
            s_isBackgroundInitialized = true;
            await LoadContentAsync();
        }
        else if (bgType == BackgroundImageType.Custom)
        {
            if (!s_isBackgroundInitialized || typeChanged)
            {
                await Task.WhenAll(LoadCustomBackgroundAsync(), LoadContentAsync());
                s_isBackgroundInitialized = true;
            }
            else
            {

                string? ext = customPath != null ? Path.GetExtension(customPath).ToLowerInvariant() : null;
                bool isVideo = ext is ".mp4" or ".webm" or ".wmv" or ".avi";

                if (isVideo)
                {

                    await Task.WhenAll(LoadCustomBackgroundAsync(), LoadContentAsync());
                }
                else
                {

                    if (_mainView != null)
                    {
                        _mainView.LauncherBackgroundImage.Opacity = 1;
                    }

                    await LoadContentAsync();
                }
            }
        }
        else if (bgType == BackgroundImageType.LauncherOfficialLauncher)
        {
            if (!s_isBackgroundInitialized)
            {
                await Task.WhenAll(LoadBackgroundAsync(), LoadContentAsync());
                s_isBackgroundInitialized = true;
            }
            else
            {
                if (s_isVideoMode && s_backgroundList.Exists(b => !string.IsNullOrEmpty(b.VideoUrl)))
                {
                    BgPauseBtn.Visibility = Visibility.Visible;
                    BgPauseIcon.Text = "\u275a\u275a";
                    BgIndicators.Visibility = Visibility.Collapsed;

                    if (s_currentBgIndex >= 0 && s_currentBgIndex < s_backgroundList.Count && _mainView is not null)
                    {
                        _mainView.LauncherBackgroundImage.Source = s_cachedFirstBitmap
                            ?? new BitmapImage { UriSource = new Uri(s_backgroundList[s_currentBgIndex].ImageUrl) };
                        _mainView.LauncherBackgroundImage.Opacity = 1;
                    }

                    PlayCurrentVideo();
                }
                else
                {
                    s_isVideoMode = false;
                    RestoreBackground();
                    LoadBgIndicators();
                    StartBgAutoSwitch();
                }

                await LoadContentAsync();
            }
        }

        if (_gameProcessCheckTimer == null)
        {
            _gameProcessCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _gameProcessCheckTimer.Tick += GameProcessCheckTimer_Tick;
        }

        _gameProcessCheckTimer.Start();
        UpdateLaunchButtonState();
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        _isPageActive = false;
        HideBackground();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CleanupPage();
    }

    private void HideBackground()
    {
        if (_mainView != null)
        {
            if (_bgSlideStoryboard != null)
            {
                _bgSlideStoryboard.Stop();
                _bgSlideStoryboard = null;
            }
            if (_videoTransitionStoryboard != null)
            {
                _videoTransitionStoryboard.Stop();
                _videoTransitionStoryboard = null;
            }

            if (_mainView.LauncherBackgroundImage.RenderTransform is CompositeTransform imgTransform)
            {
                imgTransform.TranslateX = 0;
            }
            if (_mainView.LauncherBackgroundImageOld.RenderTransform is CompositeTransform oldTransform)
            {
                oldTransform.TranslateX = 0;
            }

            CleanupBlurLayer();

            _mainView.LauncherBackgroundImage.Opacity = 0;
            _mainView.LauncherBackgroundImageOld.Opacity = 0;
            _mainView.LauncherBackgroundVideo.Opacity = 0;

            if (_mainView.FindName("MainBackdropBorder") is Border backdrop)
            {
                backdrop.Opacity = 0;
                backdrop.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void CleanupPage()
    {
        if (_bannerTimer != null)
        {
            _bannerTimer.Stop();
            _bannerTimer.Tick -= OnBannerTimerTick;
            _bannerTimer = null;
        }

        if (_bgTimer != null)
        {
            _bgTimer.Stop();
            _bgTimer.Tick -= OnBgTimerTick;
            _bgTimer = null;
        }

        if (_gameProcessCheckTimer != null)
        {
            _gameProcessCheckTimer.Stop();
            _gameProcessCheckTimer.Tick -= GameProcessCheckTimer_Tick;
            _gameProcessCheckTimer = null;
        }

        if (_bgSlideStoryboard != null)
        {
            _bgSlideStoryboard.Stop();
            _bgSlideStoryboard = null;
        }
        if (_videoTransitionStoryboard != null)
        {
            _videoTransitionStoryboard.Stop();
            _videoTransitionStoryboard = null;
        }

        StopVideo();
        CleanupBlurLayer();

        if (_mainView != null)
        {
            if (_mainView.LauncherBackgroundImage.RenderTransform is CompositeTransform imgTransform2)
            {
                imgTransform2.TranslateX = 0;
            }
            if (_mainView.LauncherBackgroundImageOld.RenderTransform is CompositeTransform oldTransform2)
            {
                oldTransform2.TranslateX = 0;
            }

            _mainView.LauncherBackgroundImage.Opacity = 0;
            _mainView.LauncherBackgroundImageOld.Opacity = 0;
            _mainView.LauncherBackgroundVideo.Opacity = 0;
            _mainView.LauncherBackgroundTheme.Opacity = 0;
            _mainView.LauncherBackgroundTheme.Source = null;

            if (_mainView.FindName("MainBackdropBorder") is Border backdrop)
            {
                backdrop.Opacity = 0;
                backdrop.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void RestoreBackground()
    {
        if (_mainView == null)
        {
            return;
        }

        if (s_lastBackgroundType == BackgroundImageType.Custom)
        {
            _mainView.LauncherBackgroundImage.Opacity = 1;
            return;
        }

        if (s_backgroundList.Count == 0)
        {
            return;
        }

        if (_suppressStaticImage)
        {
            return;
        }

        if (s_cachedFirstBitmap != null)
        {
            _mainView.LauncherBackgroundImage.Source = s_cachedFirstBitmap;
            _mainView.LauncherBackgroundImage.Opacity = 1;
            return;
        }

        if (s_currentBgIndex >= 0 && s_currentBgIndex < s_backgroundList.Count)
        {
            _mainView.LauncherBackgroundImage.Source = new BitmapImage { UriSource = new Uri(s_backgroundList[s_currentBgIndex].ImageUrl) };
            _mainView.LauncherBackgroundImage.Opacity = 1;
        }
    }

    private void ResetBackgroundState()
    {
        s_isBackgroundInitialized = false;
        s_dataInitialized = false;
        s_cachedFirstBitmap = null;
        s_backgroundList.Clear();
        s_bgImageCache.Clear();
        s_themeImageCache.Clear();
        s_currentBgIndex = 0;
        _bgTimer?.Stop();
        _bgTimer = null;
        StopVideo();
        s_isVideoMode = false;

        if (_mainView != null)
        {
            _mainView.LauncherBackgroundImage.Source = null;
            _mainView.LauncherBackgroundImage.Opacity = 0;
            _mainView.LauncherBackgroundImageOld.Source = null;
            _mainView.LauncherBackgroundImageOld.Opacity = 0;
            _mainView.LauncherBackgroundVideo.Opacity = 0;
            _mainView.LauncherBackgroundTheme.Source = null;
            _mainView.LauncherBackgroundTheme.Opacity = 0;
            BgIndicators.Children.Clear();
            BgIndicators.Visibility = Visibility.Collapsed;
            BgPauseBtn.Visibility = Visibility.Collapsed;
        }
    }

    private void CleanupBlurLayer()
    {
        if (_blurVisual != null && _mainView != null)
        {
            ElementCompositionPreview.SetElementChildVisual(_mainView.LauncherBackgroundImage, null);
        }

        _blurBrush?.Dispose();
        _blurBrush = null;
        _blurVisual?.Dispose();
        _blurVisual = null;
    }

    private void StopVideo()
    {
        if (_mainView == null)
        {
            return;
        }

        _mainView.LauncherBackgroundVideo.Opacity = 0;
        _mainView.LauncherBackgroundTheme.Opacity = 0;

        try
        {
            if (_videoWebView2Ready && _mainView.LauncherBackgroundVideo.CoreWebView2 is not null)
            {
                _mainView.LauncherBackgroundVideo.CoreWebView2.ExecuteScriptAsync(
                    "document.querySelectorAll('video').forEach(v=>{v.pause();v.removeAttribute('src');v.load()})").AsTask().ContinueWith(_ => { }, TaskScheduler.Default);
                _mainView.LauncherBackgroundVideo.CoreWebView2.NavigateToString("<html><body style='background:transparent'></body></html>");
            }
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "Video cleanup failed", "LauncherHomePage",
                [("Error", ex.Message)]));
        }
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(child);
        while (parent != null)
        {
            if (parent is T result)
            {
                return result;
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }

    private sealed class BackgroundInfo
    {
        public string ImageUrl { get; set; } = "";

        public string VideoUrl { get; set; } = "";

        public string ThemeUrl { get; set; } = "";
    }

    private sealed class BannerData
    {
        public string ImageUrl { get; set; } = "";

        public string Link { get; set; } = "";
    }

    private sealed class PostData
    {
        public string Title { get; set; } = "";

        public string Date { get; set; } = "";

        public string Link { get; set; } = "";
    }
}
