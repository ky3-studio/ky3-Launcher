using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using kyxsan.UI.Xaml.Control;
using kyxsan.UI.Content;
using kyxsan.Core;
using kyxsan.ViewModel.LauncherHome;
using kyxsan.ViewModel.Game;
using kyxsan.ViewModel.User;
using kyxsan.Service;
using kyxsan.Service.BackgroundImage;
using kyxsan.Service.User;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.Game.Package;
using kyxsan.Model.Entity;
using kyxsan.Factory.ContentDialog;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using kyxsan.Service.Game;

#pragma warning disable CA1826

namespace kyxsan.UI.Xaml.View.Page;

[SuppressMessage("", "CA1001")]
internal sealed partial class LauncherHomePage : ScopedPage
{
    private static readonly string BgCacheDir = Path.Combine(kyxsanRuntime.LocalCacheDirectory, "BgCache");
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
    private MediaPlayer? _mediaPlayer;
    private Storyboard? _bgSlideStoryboard;
    private Storyboard? _videoTransitionStoryboard;
    private SpriteVisual? _blurVisual;
    private CompositionEffectBrush? _blurBrush;
    private static bool s_isBackgroundInitialized;
    private bool _isPageActive;
    private AppOptions? _appOptions;
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
        }

        BackgroundImageType bgType = _appOptions?.BackgroundImageType.Value ?? BackgroundImageType.kyxsanOfficialLauncher;
        string? customPath = _appOptions?.BackgroundImageCustomPath.Value;

        HomeCardPanel.Visibility = _appOptions?.HomePageCardVisible.Value == false ? Visibility.Collapsed : Visibility.Visible;
        BgIndicatorPanel.Visibility = _appOptions?.HomePageIndicatorVisible.Value == false ? Visibility.Collapsed : Visibility.Visible;

        bool typeChanged = s_isBackgroundInitialized && (s_lastBackgroundType != bgType || (bgType == BackgroundImageType.Custom && s_lastCustomPath != customPath));

        bool currentShowDynamic = _appOptions?.BackgroundShowDynamic.Value ?? true;
        bool currentShowStatic = _appOptions?.BackgroundShowStatic.Value ?? true;
        bool filterChanged = bgType == BackgroundImageType.kyxsanOfficialLauncher
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
        _suppressStaticImage = bgType == BackgroundImageType.kyxsanOfficialLauncher
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

            if (!typeChanged && bgType == BackgroundImageType.kyxsanOfficialLauncher && s_cachedFirstBitmap != null && !_suppressStaticImage && !s_isVideoMode)
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
        else if (bgType == BackgroundImageType.kyxsanOfficialLauncher)
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

        _gameProcessCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _gameProcessCheckTimer.Tick += GameProcessCheckTimer_Tick;
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
        _bannerTimer?.Stop();
        _bannerTimer = null;
        _bgTimer?.Stop();
        _bgTimer = null;
        _gameProcessCheckTimer?.Stop();
        _gameProcessCheckTimer = null;
        
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
        MediaPlayer? player = _mediaPlayer;
        _mediaPlayer = null;

        if (_mainView != null)
        {
            _mainView.LauncherBackgroundVideo.Opacity = 0;
            _mainView.LauncherBackgroundTheme.Opacity = 0;
            try { _mainView.LauncherBackgroundVideo.SetMediaPlayer(null); } catch { }
        }

        if (player != null)
        {
            _ = Task.Run(() =>
            {
                try { player.Pause(); } catch { }
                try { player.Dispose(); } catch { }
            });
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

    private async Task LoadBackgroundAsync()
    {
        string? cachedFirstUrl = s_backgroundList.Count > 0 ? s_backgroundList[0].ImageUrl : null;
        bool shownFromCache = s_cachedFirstBitmap != null || await TryShowCachedBackgroundAsync();
        if (shownFromCache && cachedFirstUrl == null && s_backgroundList.Count > 0)
        {
            cachedFirstUrl = s_backgroundList[0].ImageUrl;
        }

        try
        {
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(10);
            string response = await client.GetStringAsync(
                "https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id=jGHBHlcOq1&language=zh-cn");

            try
            {
                Directory.CreateDirectory(BgCacheDir);
                await File.WriteAllTextAsync(Path.Combine(BgCacheDir, "api.json"), response);
            }
            catch { }

            List<BackgroundInfo> freshList = ParseBackgroundList(response);
            FilterBackgroundsByType(freshList);

            bool backgroundChanged = freshList.Count > 0 &&
                (cachedFirstUrl == null || cachedFirstUrl != freshList[0].ImageUrl);

            s_backgroundList.Clear();
            s_backgroundList.AddRange(freshList);

            if (backgroundChanged)
            {
                s_bgImageCache.Clear();
                s_themeImageCache.Clear();
                s_cachedFirstBitmap = null;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                if (s_backgroundList.Count > 0)
                {
                    if (!shownFromCache || backgroundChanged)
                    {
                        ShowBackground(0);
                    }

                    LoadBgIndicators();
                    StartBgAutoSwitch();
                    PreloadAndCacheBackgroundImages();
                }
            });
        }
        catch
        {
        }
    }

    private static List<BackgroundInfo> ParseBackgroundList(string apiResponse)
    {
        List<BackgroundInfo> list = [];
        try
        {
            using JsonDocument json = JsonDocument.Parse(apiResponse);
            JsonElement gameList = json.RootElement.GetProperty("data").GetProperty("game_info_list");

            foreach (JsonElement game in gameList.EnumerateArray())
            {
                string? biz = game.GetProperty("game").GetProperty("biz").GetString();
                if (biz != "hk4e_cn")
                {
                    continue;
                }

                JsonElement backgrounds = game.GetProperty("backgrounds");
                foreach (JsonElement bg in backgrounds.EnumerateArray())
                {
                    string imageUrl = "";
                    string videoUrl = "";

                    if (bg.TryGetProperty("background", out JsonElement bgInfo) &&
                        bgInfo.TryGetProperty("url", out JsonElement urlProp))
                    {
                        imageUrl = urlProp.GetString() ?? "";
                    }

                    if (bg.TryGetProperty("video", out JsonElement videoInfo) &&
                        videoInfo.TryGetProperty("url", out JsonElement videoUrlProp))
                    {
                        videoUrl = videoUrlProp.GetString() ?? "";
                    }

                    string themeUrl = "";
                    if (bg.TryGetProperty("theme", out JsonElement themeInfo) &&
                        themeInfo.TryGetProperty("url", out JsonElement themeUrlProp))
                    {
                        themeUrl = themeUrlProp.GetString() ?? "";
                    }

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        list.Add(new BackgroundInfo
                        {
                            ImageUrl = ToLosslessUrl(imageUrl),
                            VideoUrl = videoUrl,
                            ThemeUrl = themeUrl,
                        });
                    }
                }

                break;
            }
        }
        catch { }

        return list;
    }

    private static string ToLosslessUrl(string url)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("mihoyo.com"))
        {
            return url;
        }

        return url + "?x-oss-process=image/format,png";
    }

    private async Task LoadCustomBackgroundAsync()
    {
        string? customPath = _appOptions?.BackgroundImageCustomPath.Value;
        if (string.IsNullOrEmpty(customPath) || !File.Exists(customPath) || _mainView == null)
        {
            return;
        }

        try
        {
            string ext = Path.GetExtension(customPath).ToLowerInvariant();
            bool isVideo = ext is ".mp4" or ".webm" or ".wmv" or ".avi";

            if (isVideo)
            {
                StopVideo();
                _mediaPlayer = new MediaPlayer
                {
                    IsLoopingEnabled = true,
                    IsMuted = true,
                };
                _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(customPath));
                _mainView.LauncherBackgroundVideo.SetMediaPlayer(_mediaPlayer);
                _mainView.LauncherBackgroundVideo.Opacity = 1;
                _mainView.LauncherBackgroundImage.Opacity = 0;
                _mediaPlayer.Play();
                s_isVideoMode = true;
            }
            else
            {
                byte[] data = await File.ReadAllBytesAsync(customPath);
                BitmapImage? bmp = await CreateBitmapFromData(data);
                if (bmp != null && _mainView != null && _isPageActive)
                {
                    _mainView.LauncherBackgroundImage.Source = bmp;
                    _mainView.LauncherBackgroundImage.Opacity = 1;
                }
            }
        }
        catch
        {
        }
    }

    private async Task<bool> TryShowCachedBackgroundAsync()
    {
        try
        {
            bool currentShowDynamic = _appOptions?.BackgroundShowDynamic.Value ?? true;
            bool currentShowStatic = _appOptions?.BackgroundShowStatic.Value ?? true;
            bool filterChanged = s_lastShowDynamic != currentShowDynamic || s_lastShowStatic != currentShowStatic;

            if (filterChanged)
            {
                s_dataInitialized = false;
                s_cachedFirstBitmap = null;
                s_backgroundList.Clear();
                s_bgImageCache.Clear();
                s_themeImageCache.Clear();
                s_lastShowDynamic = currentShowDynamic;
                s_lastShowStatic = currentShowStatic;
            }

            if (s_dataInitialized && s_backgroundList.Count > 0 && s_bgImageCache.ContainsKey(0))
            {
                if (s_cachedFirstBitmap == null)
                {
                    s_cachedFirstBitmap = await CreateBitmapFromData(s_bgImageCache[0]);
                }

                if (s_cachedFirstBitmap != null && _mainView != null && _isPageActive && !_suppressStaticImage)
                {
                    _mainView.LauncherBackgroundImage.Source = s_cachedFirstBitmap;
                    _mainView.LauncherBackgroundImage.Opacity = 1;
                }

                return s_cachedFirstBitmap != null;
            }

            string apiCachePath = Path.Combine(BgCacheDir, "api.json");
            if (!File.Exists(apiCachePath))
            {
                return false;
            }

            string cachedResponse = await File.ReadAllTextAsync(apiCachePath);
            List<BackgroundInfo> cachedList = ParseBackgroundList(cachedResponse);
            FilterBackgroundsByType(cachedList);
            if (cachedList.Count == 0)
            {
                return false;
            }

            string imgCachePath = GetBgImageCachePath(cachedList[0].ImageUrl);
            if (!File.Exists(imgCachePath))
            {
                return false;
            }

            s_backgroundList.Clear();
            s_backgroundList.AddRange(cachedList);
            s_dataInitialized = true;

            s_cachedFirstBitmap = new BitmapImage { UriSource = new Uri(imgCachePath) };

            if (_mainView != null && _isPageActive && !_suppressStaticImage)
            {
                _mainView.LauncherBackgroundImage.Source = s_cachedFirstBitmap;
                _mainView.LauncherBackgroundImage.Opacity = 1;
            }

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < cachedList.Count; i++)
                {
                    try
                    {
                        string path = GetBgImageCachePath(cachedList[i].ImageUrl);
                        if (File.Exists(path))
                        {
                            s_bgImageCache[i] = await File.ReadAllBytesAsync(path);
                        }
                    }
                    catch { }
                }
            });

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GetBgImageCachePath(string imageUrl)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(imageUrl));
        return Path.Combine(BgCacheDir, $"bg_{Convert.ToHexString(hash)}.cache");
    }

    private static async Task<BitmapImage?> CreateBitmapFromData(byte[] data)
    {
        try
        {
            BitmapImage bmp = new();
            using InMemoryRandomAccessStream stream = new();
            DataWriter writer = new(stream.GetOutputStreamAt(0));
            writer.WriteBytes(data);
            await writer.StoreAsync();
            writer.DetachStream();
            stream.Seek(0);
            await bmp.SetSourceAsync(stream);
            return bmp;
        }
        catch
        {
            return null;
        }
    }

    private void LoadBgIndicators()
    {
        if (s_backgroundList.Count <= 1)
        {
            return;
        }

        BgIndicators.Visibility = Visibility.Visible;
        BgIndicators.Children.Clear();

        for (int i = 0; i < s_backgroundList.Count; i++)
        {
            Border dot = new()
            {
                Width = 10,
                Height = 10,
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(i == 0 ? Colors.White : Color.FromArgb(100, 255, 255, 255)),
                Margin = new Thickness(4, 0, 4, 0),
            };
            int index = i;
            dot.PointerPressed += (_, _) => ShowBackground(index);
            BgIndicators.Children.Add(dot);
        }
    }

    private void StartBgAutoSwitch()
    {
        bool showDynamic = _appOptions?.BackgroundShowDynamic.Value ?? true;
        bool showStatic = _appOptions?.BackgroundShowStatic.Value ?? true;

        bool hasAnyVideo = s_backgroundList.Exists(b => !string.IsNullOrEmpty(b.VideoUrl));

        if (showDynamic && s_currentBgIndex >= 0 && s_currentBgIndex < s_backgroundList.Count
            && !string.IsNullOrEmpty(s_backgroundList[s_currentBgIndex].VideoUrl))
        {
            BgPauseBtn.Visibility = Visibility.Visible;
            BgPauseIcon.Text = "\u25b6";
        }

        if (showDynamic && !showStatic && hasAnyVideo && s_backgroundList.Count >= 1)
        {
            _bgTimer?.Stop();
            s_isVideoMode = true;
            BgPauseIcon.Text = "\u275a\u275a";
            PlayCurrentVideo();
            return;
        }

        if (s_backgroundList.Count <= 1)
        {
            return;
        }

        s_isVideoMode = false;
        int intervalSeconds = _appOptions?.BackgroundSwitchInterval.Value ?? 8;
        _bgTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(intervalSeconds) };
        _bgTimer.Tick += (_, _) => ShowBackground((s_currentBgIndex + 1) % s_backgroundList.Count);
        _bgTimer.Start();
    }

    private async void PreloadAndCacheBackgroundImages()
    {
        try
        {
            Directory.CreateDirectory(BgCacheDir);
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(20);
            for (int i = 0; i < s_backgroundList.Count; i++)
            {
                if (s_bgImageCache.ContainsKey(i))
                {
                    SaveImageToDiskCache(i);
                }
                else
                {
                    try
                    {
                        byte[] data = await client.GetByteArrayAsync(s_backgroundList[i].ImageUrl);
                        s_bgImageCache[i] = data;
                        SaveImageToDiskCache(i);
                    }
                    catch
                    {
                    }
                }

                string themeUrl = s_backgroundList[i].ThemeUrl;
                if (!string.IsNullOrEmpty(themeUrl) && !s_themeImageCache.ContainsKey(i))
                {
                    try
                    {
                        string themeCachePath = GetBgImageCachePath(themeUrl);
                        if (File.Exists(themeCachePath))
                        {
                            s_themeImageCache[i] = await File.ReadAllBytesAsync(themeCachePath);
                        }
                        else
                        {
                            byte[] themeData = await client.GetByteArrayAsync(themeUrl);
                            s_themeImageCache[i] = themeData;
                            File.WriteAllBytes(themeCachePath, themeData);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            s_dataInitialized = true;

            CleanupStaleBgCache();
        }
        catch
        {
        }
    }

    private static void CleanupStaleBgCache()
    {
        try
        {
            if (!Directory.Exists(BgCacheDir)) return;

            HashSet<string> validFiles = new(StringComparer.OrdinalIgnoreCase) { "api.json" };
            foreach (BackgroundInfo bg in s_backgroundList)
            {
                validFiles.Add(Path.GetFileName(GetBgImageCachePath(bg.ImageUrl)));
                if (!string.IsNullOrEmpty(bg.ThemeUrl))
                {
                    validFiles.Add(Path.GetFileName(GetBgImageCachePath(bg.ThemeUrl)));
                }
            }

            foreach (string file in Directory.GetFiles(BgCacheDir))
            {
                string fileName = Path.GetFileName(file);
                if (!validFiles.Contains(fileName))
                {
                    try { File.Delete(file); } catch { }
                }
            }
        }
        catch { }
    }

    private static void SaveImageToDiskCache(int index)
    {
        try
        {
            if (!s_bgImageCache.TryGetValue(index, out byte[]? data) || index >= s_backgroundList.Count)
            {
                return;
            }

            string path = GetBgImageCachePath(s_backgroundList[index].ImageUrl);
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, data);
            }
        }
        catch { }
    }

    private async void ShowBackground(int index, bool forceNoAnimation = false)
    {
        if (index < 0 || index >= s_backgroundList.Count || _mainView == null)
        {
            return;
        }

        bool isFirst = _mainView.LauncherBackgroundImage.Source == null;
        s_currentBgIndex = index;
        UpdateBgIndicators(index);

        bool showDynamic = _appOptions?.BackgroundShowDynamic.Value ?? true;
        bool currentHasVideo = !string.IsNullOrEmpty(s_backgroundList[index].VideoUrl) && showDynamic;
        BgPauseBtn.Visibility = currentHasVideo ? Visibility.Visible : Visibility.Collapsed;
        if (currentHasVideo && !s_isVideoMode)
        {
            BgPauseIcon.Text = "\u25b6";
        }

        if (forceNoAnimation || isFirst)
        {
            BitmapImage? bmp = await GetOrDownloadBitmap(index);
            if (bmp != null && _mainView != null)
            {
                if (index == 0)
                {
                    s_cachedFirstBitmap = bmp;
                }

                _mainView.LauncherBackgroundImage.Source = bmp;
                if (_isPageActive && !_suppressStaticImage)
                {
                    _mainView.LauncherBackgroundImage.Opacity = 1;
                }
            }
        }
        else
        {
            BitmapImage? newBmp = await GetOrDownloadBitmap(index);
            if (newBmp != null && s_currentBgIndex == index && _mainView != null && _isPageActive)
            {
                RunSlideAnimation(newBmp);
            }
        }

        if (_mainView != null)
        {
            _mainView.LauncherBackgroundTheme.Opacity = 0;
        }
    }

    private static async Task<BitmapImage?> GetOrDownloadBitmap(int index)
    {
        if (s_bgImageCache.TryGetValue(index, out byte[]? cached))
        {
            return await CreateBitmapFromData(cached);
        }

        if (index < s_backgroundList.Count)
        {
            string diskPath = GetBgImageCachePath(s_backgroundList[index].ImageUrl);
            if (File.Exists(diskPath))
            {
                try
                {
                    byte[] diskData = await File.ReadAllBytesAsync(diskPath);
                    s_bgImageCache[index] = diskData;
                    return await CreateBitmapFromData(diskData);
                }
                catch { }
            }
        }

        try
        {
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(15);
            byte[] data = await client.GetByteArrayAsync(s_backgroundList[index].ImageUrl);
            s_bgImageCache[index] = data;
            SaveImageToDiskCache(index);
            return await CreateBitmapFromData(data);
        }
        catch
        {
            return null;
        }
    }

    private async Task UpdateThemeOverlay(int index)
    {
        if (_mainView == null || index < 0 || index >= s_backgroundList.Count)
        {
            return;
        }

        string themeUrl = s_backgroundList[index].ThemeUrl;
        if (string.IsNullOrEmpty(themeUrl))
        {
            _mainView.LauncherBackgroundTheme.Opacity = 0;
            _mainView.LauncherBackgroundTheme.Source = null;
            return;
        }

        try
        {
            BitmapImage? themeBmp = null;
            if (s_themeImageCache.TryGetValue(index, out byte[]? cached))
            {
                themeBmp = await CreateBitmapFromData(cached);
            }
            else
            {
                string diskPath = GetBgImageCachePath(themeUrl);
                if (File.Exists(diskPath))
                {
                    byte[] diskData = await File.ReadAllBytesAsync(diskPath);
                    s_themeImageCache[index] = diskData;
                    themeBmp = await CreateBitmapFromData(diskData);
                }
                else
                {
                    using HttpClient client = new();
                    client.Timeout = TimeSpan.FromSeconds(15);
                    byte[] data = await client.GetByteArrayAsync(themeUrl);
                    s_themeImageCache[index] = data;
                    Directory.CreateDirectory(BgCacheDir);
                    File.WriteAllBytes(diskPath, data);
                    themeBmp = await CreateBitmapFromData(data);
                }
            }

            if (themeBmp != null && _mainView != null && s_currentBgIndex == index)
            {
                _mainView.LauncherBackgroundTheme.Source = themeBmp;
                _mainView.LauncherBackgroundTheme.Opacity = 1;
            }
        }
        catch
        {
            if (_mainView != null)
            {
                _mainView.LauncherBackgroundTheme.Opacity = 0;
            }
        }
    }

    private void RunSlideAnimation(BitmapImage newSource)
    {
        if (_mainView == null)
        {
            return;
        }

        _bgSlideStoryboard?.Stop();
        _bgSlideStoryboard = null;

        double width = _mainView.ActualWidth + 100;

        CompositeTransform oldTransform = (CompositeTransform)_mainView.LauncherBackgroundImageOld.RenderTransform;
        CompositeTransform newTransform = (CompositeTransform)_mainView.LauncherBackgroundImage.RenderTransform;

        oldTransform.TranslateX = 0;
        newTransform.TranslateX = 0;

        _mainView.LauncherBackgroundImageOld.Source = _mainView.LauncherBackgroundImage.Source;
        _mainView.LauncherBackgroundImageOld.Opacity = 1;

        _mainView.LauncherBackgroundImage.Source = newSource;
        _mainView.LauncherBackgroundImage.Opacity = 1;

        newTransform.TranslateX = width;

        Storyboard sb = new();

        DoubleAnimation slideOut = new()
        {
            From = 0,
            To = -width,
            Duration = new Duration(TimeSpan.FromMilliseconds(600)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        };
        Storyboard.SetTarget(slideOut, oldTransform);
        Storyboard.SetTargetProperty(slideOut, "TranslateX");

        DoubleAnimation slideIn = new()
        {
            From = width,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(600)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
        };
        Storyboard.SetTarget(slideIn, newTransform);
        Storyboard.SetTargetProperty(slideIn, "TranslateX");

        sb.Children.Add(slideOut);
        sb.Children.Add(slideIn);
        sb.Completed += (_, _) =>
        {
            sb.Stop();
            _mainView.LauncherBackgroundImageOld.Opacity = 0;
            oldTransform.TranslateX = 0;
            newTransform.TranslateX = 0;
        };
        _bgSlideStoryboard = sb;
        sb.Begin();
    }

    private void EnsureBlurLayer()
    {
        if (_blurVisual != null || _mainView == null)
        {
            return;
        }

        Visual hostVisual = ElementCompositionPreview.GetElementVisual(_mainView.LauncherBackgroundImage);
        Compositor compositor = hostVisual.Compositor;

        GaussianBlurEffect blurEffect = new()
        {
            Name = "Blur",
            BlurAmount = 0f,
            BorderMode = EffectBorderMode.Hard,
            Source = new CompositionEffectSourceParameter("backdrop"),
        };

        CompositionEffectFactory factory = compositor.CreateEffectFactory(blurEffect, (string[])["Blur.BlurAmount"]);
        CompositionBackdropBrush backdropBrush = compositor.CreateBackdropBrush();
        _blurBrush = factory.CreateBrush();
        _blurBrush.SetSourceParameter("backdrop", backdropBrush);

        _blurVisual = compositor.CreateSpriteVisual();
        _blurVisual.Brush = _blurBrush;
        _blurVisual.RelativeSizeAdjustment = Vector2.One;

        ElementCompositionPreview.SetElementChildVisual(_mainView.LauncherBackgroundImage, _blurVisual);
    }

    private void AnimateVideoIn()
    {
        if (_mainView == null)
        {
            return;
        }

        _videoTransitionStoryboard?.Stop();
        _videoTransitionStoryboard = null;

        EnsureBlurLayer();

        if (_blurBrush != null && _blurVisual != null)
        {
            Compositor compositor = _blurVisual.Compositor;
            float peakBlur = 12f;
            TimeSpan halfDuration = TimeSpan.FromMilliseconds(300);

            ScalarKeyFrameAnimation blurUp = compositor.CreateScalarKeyFrameAnimation();
            blurUp.InsertKeyFrame(0f, 0f);
            blurUp.InsertKeyFrame(1f, peakBlur);
            blurUp.Duration = halfDuration;

            _blurBrush.StartAnimation("Blur.BlurAmount", blurUp);

            DispatcherTimer delayTimer = new() { Interval = halfDuration };
            delayTimer.Tick += (_, _) =>
            {
                delayTimer.Stop();

                if (_mainView == null || _blurVisual == null)
                {
                    return;
                }

                _mainView.LauncherBackgroundVideo.Opacity = 1;

                ScalarKeyFrameAnimation blurDown = _blurVisual.Compositor.CreateScalarKeyFrameAnimation();
                blurDown.InsertKeyFrame(0f, peakBlur);
                blurDown.InsertKeyFrame(1f, 0f);
                blurDown.Duration = halfDuration;
                _blurBrush?.StartAnimation("Blur.BlurAmount", blurDown);
            };
            delayTimer.Start();
        }
        else
        {
            _mainView.LauncherBackgroundVideo.Opacity = 1;
        }
    }

    private void AnimateVideoOut(Action? onCompleted)
    {
        if (_mainView == null)
        {
            onCompleted?.Invoke();
            return;
        }

        _videoTransitionStoryboard?.Stop();
        _videoTransitionStoryboard = null;

        EnsureBlurLayer();

        if (_blurBrush != null && _blurVisual != null)
        {
            Compositor compositor = _blurVisual.Compositor;
            float peakBlur = 12f;
            TimeSpan halfDuration = TimeSpan.FromMilliseconds(300);

            ScalarKeyFrameAnimation blurUp = compositor.CreateScalarKeyFrameAnimation();
            blurUp.InsertKeyFrame(0f, 0f);
            blurUp.InsertKeyFrame(1f, peakBlur);
            blurUp.Duration = halfDuration;

            _blurBrush.StartAnimation("Blur.BlurAmount", blurUp);

            DispatcherTimer delayTimer = new() { Interval = halfDuration };
            delayTimer.Tick += (_, _) =>
            {
                delayTimer.Stop();

                if (_mainView == null || _blurVisual == null)
                {
                    onCompleted?.Invoke();
                    return;
                }

                _mainView.LauncherBackgroundVideo.Opacity = 0;
                _mainView.LauncherBackgroundTheme.Opacity = 0;

                ScalarKeyFrameAnimation blurDown = _blurVisual.Compositor.CreateScalarKeyFrameAnimation();
                blurDown.InsertKeyFrame(0f, peakBlur);
                blurDown.InsertKeyFrame(1f, 0f);
                blurDown.Duration = halfDuration;

                CompositionScopedBatch batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                _blurBrush?.StartAnimation("Blur.BlurAmount", blurDown);
                batch.End();
                batch.Completed += (_, _) =>
                {
                    DispatcherQueue?.TryEnqueue(() => onCompleted?.Invoke());
                };
            };
            delayTimer.Start();
        }
        else
        {
            onCompleted?.Invoke();
        }
    }

    private void UpdateBgIndicators(int index)
    {
        for (int i = 0; i < BgIndicators.Children.Count; i++)
        {
            if (BgIndicators.Children[i] is Border dot)
            {
                dot.Background = new SolidColorBrush(
                    i == index ? Colors.White : Color.FromArgb(100, 255, 255, 255));
            }
        }
    }

    private void Page_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
    }

    private void Page_PointerExited(object sender, PointerRoutedEventArgs e)
    {
    }

    private void BgPausePlay_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_mainView == null)
        {
            return;
        }

        if (s_isVideoMode)
        {
            s_isVideoMode = false;
            BgPauseIcon.Text = "\u25b6";
            AnimateVideoOut(() =>
            {
                StopVideo();
                LoadBgIndicators();
                BgIndicators.Visibility = Visibility.Visible;
                StartBgAutoSwitch();
            });
        }
        else
        {
            _bgTimer?.Stop();
            s_isVideoMode = true;
            BgPauseIcon.Text = "\u275a\u275a";
            BgIndicators.Visibility = Visibility.Collapsed;
            PlayCurrentVideo();
        }
    }

    private void PlayCurrentVideo()
    {
        if (_mainView == null || s_currentBgIndex < 0 || s_currentBgIndex >= s_backgroundList.Count)
        {
            return;
        }

        BackgroundInfo bg = s_backgroundList[s_currentBgIndex];
        if (string.IsNullOrEmpty(bg.VideoUrl))
        {
            return;
        }

        StopVideo();
        try
        {
            _mediaPlayer = new MediaPlayer
            {
                IsLoopingEnabled = true,
                IsMuted = true,
            };
            _mediaPlayer.MediaOpened += (_, _) =>
            {
                DispatcherQueue?.TryEnqueue(() =>
                {
                    if (_mainView != null && s_isVideoMode)
                    {
                        _ = UpdateThemeOverlay(s_currentBgIndex);
                        AnimateVideoIn();
                    }
                });
            };
            _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(bg.VideoUrl));
            _mainView.LauncherBackgroundVideo.SetMediaPlayer(_mediaPlayer);
            _mediaPlayer.Play();
        }
        catch
        {
            StopVideo();
        }
    }

    private void Banner_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        BannerIndicators.Opacity = 1;
    }

    private void Banner_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        BannerIndicators.Opacity = 0;
    }

    private async Task LoadContentAsync()
    {
        try
        {
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(10);
            string response = await client.GetStringAsync(
                "https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getGameContent?launcher_id=jGHBHlcOq1&game_id=1Z8W5NHUQb&language=zh-cn");

            using JsonDocument json = JsonDocument.Parse(response);
            JsonElement content = json.RootElement.GetProperty("data").GetProperty("content");

            if (content.TryGetProperty("banners", out JsonElement banners))
            {
                foreach (JsonElement banner in banners.EnumerateArray())
                {
                    JsonElement image = banner.GetProperty("image");
                    _bannerList.Add(new BannerData
                    {
                        ImageUrl = image.GetProperty("url").GetString() ?? "",
                        Link = image.GetProperty("link").GetString() ?? "",
                    });
                }
            }

            if (content.TryGetProperty("posts", out JsonElement posts))
            {
                foreach (JsonElement post in posts.EnumerateArray())
                {
                    string? type = post.GetProperty("type").GetString();
                    PostData item = new()
                    {
                        Title = post.GetProperty("title").GetString() ?? "",
                        Date = post.GetProperty("date").GetString() ?? "",
                        Link = post.GetProperty("link").GetString() ?? "",
                    };

                    switch (type)
                    {
                        case "POST_TYPE_ACTIVITY":
                            _activityList.Add(item);
                            break;
                        case "POST_TYPE_ANNOUNCE":
                            _announceList.Add(item);
                            break;
                        case "POST_TYPE_INFO":
                            _infoList.Add(item);
                            break;
                    }
                }
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                LoadBanners();
                ShowTab("activity");
            });
        }
        catch
        {
        }
    }

    private void LoadBanners()
    {
        if (_bannerList.Count == 0)
        {
            return;
        }

        ShowBanner(0);

        if (_bannerList.Count > 1)
        {
            BannerIndicators.Visibility = Visibility.Visible;
            BannerIndicators.Children.Clear();
            for (int i = 0; i < _bannerList.Count; i++)
            {
                Border dot = new()
                {
                    Width = 8,
                    Height = 8,
                    CornerRadius = new CornerRadius(4),
                    Background = new SolidColorBrush(i == 0 ? Colors.White : Color.FromArgb(100, 255, 255, 255)),
                    Margin = new Thickness(3, 0, 3, 0),
                };
                int index = i;
                dot.PointerPressed += (_, _) => ShowBanner(index);
                BannerIndicators.Children.Add(dot);
            }

            _bannerTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _bannerTimer.Tick += (_, _) => ShowBanner((_currentBannerIndex + 1) % _bannerList.Count);
            _bannerTimer.Start();
        }
    }

    private void ShowBanner(int index)
    {
        if (index < 0 || index >= _bannerList.Count)
        {
            return;
        }

        bool isFirst = _currentBannerIndex == index && BannerImage.Source == null;
        _currentBannerIndex = index;
        BannerData banner = _bannerList[index];
        BitmapImage newSource = new() { DecodePixelWidth = 720, UriSource = new Uri(banner.ImageUrl) };

        if (isFirst)
        {
            BannerImage.Source = newSource;
        }
        else
        {
            BannerImageOld.Source = BannerImage.Source;
            BannerImageOld.Opacity = 1;
            BannerImage.Opacity = 0;
            BannerImage.Source = newSource;

            Storyboard sb = new();

            DoubleAnimation fadeIn = new()
            {
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(400)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            };
            Storyboard.SetTarget(fadeIn, BannerImage);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");

            DoubleAnimation fadeOut = new()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(400)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            };
            Storyboard.SetTarget(fadeOut, BannerImageOld);
            Storyboard.SetTargetProperty(fadeOut, "Opacity");

            sb.Children.Add(fadeIn);
            sb.Children.Add(fadeOut);
            sb.Begin();
        }

        for (int i = 0; i < BannerIndicators.Children.Count; i++)
        {
            if (BannerIndicators.Children[i] is Border dot)
            {
                dot.Background = new SolidColorBrush(i == index ? Colors.White : Color.FromArgb(100, 255, 255, 255));
            }
        }
    }

    private async void Banner_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_currentBannerIndex >= 0 && _currentBannerIndex < _bannerList.Count)
        {
            string link = _bannerList[_currentBannerIndex].Link;
            if (!string.IsNullOrEmpty(link))
            {
                await Launcher.LaunchUriAsync(new Uri(link));
            }
        }
    }

    private void Tab_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is TextBlock tb && tb.Tag is string tag)
        {
            ShowTab(tag);
        }
    }

    private async void ShowTab(string tab)
    {
        if (tab == _currentTab && PostList.Children.Count > 0)
        {
            return;
        }

        _currentTab = tab;

        TabActivity.FontWeight = tab == "activity" ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;
        TabAnnounce.FontWeight = tab == "announce" ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;
        TabInfo.FontWeight = tab == "info" ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal;

        TabActivity.Foreground = new SolidColorBrush(tab == "activity" ? Colors.White : Color.FromArgb(128, 255, 255, 255));
        TabAnnounce.Foreground = new SolidColorBrush(tab == "announce" ? Colors.White : Color.FromArgb(128, 255, 255, 255));
        TabInfo.Foreground = new SolidColorBrush(tab == "info" ? Colors.White : Color.FromArgb(128, 255, 255, 255));

        double offset = tab switch
        {
            "announce" => TabAnnounce.ActualOffset.X,
            "info" => TabInfo.ActualOffset.X,
            _ => TabActivity.ActualOffset.X,
        };
        TabIndicator.Translation = new Vector3((float)offset, 0, 0);

        PostList.Opacity = 0;
        PostList.Translation = new Vector3(0, 8, 0);

        await Task.Delay(150);

        List<PostData> posts = tab switch
        {
            "announce" => _announceList,
            "info" => _infoList,
            _ => _activityList,
        };

        PostList.Children.Clear();
        foreach (PostData post in posts)
        {
            Grid row = new() { ColumnSpacing = 8 };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock title = new()
            {
                Text = post.Title,
                Foreground = new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)),
                FontSize = 13,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxLines = 1,
            };
            Grid.SetColumn(title, 0);

            TextBlock date = new()
            {
                Text = post.Date,
                Foreground = new SolidColorBrush(Color.FromArgb(150, 255, 255, 255)),
                FontSize = 13,
            };
            Grid.SetColumn(date, 1);

            row.Children.Add(title);
            row.Children.Add(date);

            string link = post.Link;
            row.PointerPressed += async (_, _) =>
            {
                if (!string.IsNullOrEmpty(link))
                {
                    await Launcher.LaunchUriAsync(new Uri(link));
                }
            };
            row.PointerEntered += (s, _) =>
            {
                if (s is Grid g && g.Children[0] is TextBlock t)
                {
                    t.Foreground = new SolidColorBrush(Colors.White);
                }
            };
            row.PointerExited += (s, _) =>
            {
                if (s is Grid g && g.Children[0] is TextBlock t)
                {
                    t.Foreground = new SolidColorBrush(Color.FromArgb(220, 255, 255, 255));
                }
            };

            PostList.Children.Add(row);
        }

        PostList.Opacity = 1;
        PostList.Translation = Vector3.Zero;
    }

    private void LaunchButton_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _isLaunchButtonHovered = true;
        LaunchButton.CenterPoint = new Vector3((float)(LaunchButton.ActualWidth / 2), (float)(LaunchButton.ActualHeight / 2), 0);
        LaunchButton.Scale = new Vector3(1.06f, 1.06f, 1);
        AnimateLaunchButtonColors(hovered: true);
    }

    private void LaunchButton_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        _isLaunchButtonHovered = false;
        LaunchButton.Scale = Vector3.One;
        UpdateLaunchButtonState();
    }

    private async void LaunchButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool isGameRunning = LaunchOptions.IsGameRunning?.Value ?? false;

            if (isGameRunning)
            {
                if (XamlRoot.XamlContext() is { } context)
                {
                    ITaskContext taskContext = context.ServiceProvider.GetRequiredService<ITaskContext>();
                    await GameLifeCycle.TryKillGameProcessAsync(taskContext).ConfigureAwait(false);
                }
            }
            else
            {
                if (XamlRoot.XamlContext() is not { } context)
                {
                    return;
                }

                LaunchGameShared shared = context.ServiceProvider.GetRequiredService<LaunchGameShared>();
                IUserService userService = context.ServiceProvider.GetRequiredService<IUserService>();
                
                UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
                await shared.DefaultLaunchExecutionAsync(new LauncherPageLaunchSupport(this, shared), userAndUid).ConfigureAwait(false);
            }
        }
        catch
        {
        }
    }

    private void GameProcessCheckTimer_Tick(object? sender, object e)
    {
        UpdateLaunchButtonState();
    }

    private void UpdateLaunchButtonState()
    {
        bool isGameRunning = LaunchOptions.IsGameRunning?.Value ?? false;

        if (isGameRunning)
        {
            LaunchButtonText.Text = SH.ViewPageHomeStopGameButton;
            PlayIconText.Text = "■";
            AnimateLaunchButtonColors(hovered: _isLaunchButtonHovered, running: true);
        }
        else
        {
            LaunchButtonText.Text = SH.ViewPageHomeLaunchGameButton;
            PlayIconText.Text = "▶";
            AnimateLaunchButtonColors(hovered: _isLaunchButtonHovered);
        }
    }

    private void AnimateLaunchButtonColors(bool hovered, bool running = false)
    {
        Color bgColor, iconBg, iconFg, textFg;

        if (running)
        {
            if (hovered)
            {
                bgColor = Color.FromArgb(230, 180, 30, 40);
                iconBg = Color.FromArgb(255, 255, 199, 41);
                iconFg = Color.FromArgb(255, 220, 53, 69);
                textFg = Colors.White;
            }
            else
            {
                bgColor = Color.FromArgb(255, 220, 53, 69);
                iconBg = Color.FromArgb(255, 255, 199, 41);
                iconFg = Color.FromArgb(255, 220, 53, 69);
                textFg = Colors.White;
            }
        }
        else
        {
            if (hovered)
            {
                bgColor = Color.FromArgb(230, 35, 35, 40);
                iconBg = Color.FromArgb(255, 255, 199, 41);
                iconFg = Color.FromArgb(255, 35, 35, 40);
                textFg = Colors.White;
            }
            else
            {
                bgColor = Color.FromArgb(255, 255, 199, 41);
                iconBg = Color.FromArgb(255, 45, 45, 50);
                iconFg = Color.FromArgb(255, 255, 199, 41);
                textFg = Color.FromArgb(255, 40, 40, 40);
            }
        }

        TimeSpan duration = TimeSpan.FromMilliseconds(180);

        AnimateBrushColor(LaunchButtonBg, bgColor, duration);
        AnimateBrushColor(PlayIconBg, iconBg, duration);
        AnimateBrushColor(PlayIconFg, iconFg, duration);
        AnimateBrushColor(LaunchTextFg, textFg, duration);
    }

    private static void AnimateBrushColor(SolidColorBrush brush, Color target, TimeSpan duration)
    {
        ColorAnimation animation = new()
        {
            To = target,
            Duration = new Duration(duration),
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, brush);
        Storyboard.SetTargetProperty(animation, "Color");

        Storyboard sb = new();
        sb.Children.Add(animation);
        sb.Begin();
    }

    private sealed class LauncherPageLaunchSupport : IViewModelSupportLaunchExecution
    {
        private readonly LauncherHomePage page;
        private readonly LaunchGameShared shared;

        public LauncherPageLaunchSupport(LauncherHomePage page, LaunchGameShared shared)
        {
            this.page = page;
            this.shared = shared;
        }

        public LaunchScheme? TargetScheme => shared.GetCurrentLaunchSchemeFromConfigurationFile(false);

        public LaunchScheme? CurrentScheme => shared.GetCurrentLaunchSchemeFromConfigurationFile(false);

        public GameAccount? GameAccount => null;

        public ValueTask<BlockDeferral<PackageConvertStatus>> CreateConvertBlockDeferralAsync()
        {
            throw new NotSupportedException("Package conversion is not supported from launcher home page.");
        }
    }

    private void FilterBackgroundsByType(List<BackgroundInfo> list)
    {
        bool showDynamic = _appOptions?.BackgroundShowDynamic.Value ?? true;
        bool showStatic = _appOptions?.BackgroundShowStatic.Value ?? true;

        if (showDynamic && showStatic)
        {
            return;
        }

        if (!showDynamic && !showStatic)
        {
            return;
        }

        if (!showStatic)
        {
            list.RemoveAll(bg => string.IsNullOrEmpty(bg.VideoUrl));
        }
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
