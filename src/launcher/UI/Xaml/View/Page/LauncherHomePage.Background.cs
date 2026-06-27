//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Web.WebView2.Core;
using Launcher.Core.IO;
using Launcher.Core.Logging;
using Launcher.Service.Constants;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage.Streams;
using Windows.UI;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LauncherHomePage
{
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
            using HttpClient client = _httpClientFactory!.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DefaultTimeoutSeconds);
            string response = await client.GetStringAsync(
                LauncherApiConstants.MiHoYoGameInfoApi);

            try
            {
                Directory.CreateDirectory(BgCacheDir);
                await File.WriteAllTextAsync(Path.Combine(BgCacheDir, "api.json"), response);
            }
            catch (Exception ex)
            {
                SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                    "Cache write failed", "LauncherHomePage",
                    [("Error", ex.Message)]));
            }

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
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
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
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "Parse background list failed", "LauncherHomePage",
                [("Error", ex.Message)]));
        }

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
                await EnsureVideoWebView2Async();
                if (_mainView != null && _videoWebView2Ready)
                {
                    string dir = Path.GetDirectoryName(customPath)!;
                    string fileName = Path.GetFileName(customPath);
                    _mainView.LauncherBackgroundVideo.CoreWebView2.SetVirtualHostNameToFolderMapping(
                        "bgcustom.local", dir, CoreWebView2HostResourceAccessKind.Allow);
                    string videoHtml = BuildVideoHtml("https://bgcustom.local/" + Uri.EscapeDataString(fileName));
                    _mainView.LauncherBackgroundVideo.CoreWebView2.NavigateToString(videoHtml);
                    _mainView.LauncherBackgroundVideo.Opacity = 1;
                    _mainView.LauncherBackgroundImage.Opacity = 0;
                    s_isVideoMode = true;
                }
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
                    catch (Exception ex)
                    {
                        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                            "Cache read failed", "LauncherHomePage",
                            [("Error", ex.Message)]));
                    }
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

    private static string GetVideoCachePath(string videoUrl)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(videoUrl));
        string ext = Path.GetExtension(new Uri(videoUrl).AbsolutePath);
        if (string.IsNullOrEmpty(ext)) ext = ".webm";
        return Path.Combine(BgCacheDir, $"bg_{Convert.ToHexString(hash)}{ext}");
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
        _bgTimer.Tick += OnBgTimerTick;
        _bgTimer.Start();
    }

    private async void PreloadAndCacheBackgroundImages()
    {
        try
        {
            Directory.CreateDirectory(BgCacheDir);
            using HttpClient client = _httpClientFactory!.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DownloadTimeoutSeconds);
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

                string videoUrl = s_backgroundList[i].VideoUrl;
                if (!string.IsNullOrEmpty(videoUrl))
                {
                    try
                    {
                        string videoCachePath = GetVideoCachePath(videoUrl);
                        if (!File.Exists(videoCachePath))
                        {
                            byte[] videoData = await client.GetByteArrayAsync(videoUrl);
                            File.WriteAllBytes(videoCachePath, videoData);
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
                if (!string.IsNullOrEmpty(bg.VideoUrl))
                {
                    validFiles.Add(Path.GetFileName(GetVideoCachePath(bg.VideoUrl)));
                }
            }

            foreach (string file in Directory.GetFiles(BgCacheDir))
            {
                string fileName = Path.GetFileName(file);
                if (!validFiles.Contains(fileName))
                {
                    FileOperationSafe.TryDelete(file);
                }
            }
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "Cache cleanup failed", "LauncherHomePage",
                [("Error", ex.Message)]));
        }
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
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "Save image to disk cache failed", "LauncherHomePage",
                [("Index", index.ToString()), ("Error", ex.Message)]));
        }
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

    private async Task<BitmapImage?> GetOrDownloadBitmap(int index)
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
                catch (Exception ex)
                {
                    SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                        "Disk cache read failed", "LauncherHomePage",
                        [("Index", index.ToString()), ("Error", ex.Message)]));
                }
            }
        }

        try
        {
            using HttpClient client = _httpClientFactory!.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.ImageDownloadTimeoutSeconds);
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
                    using HttpClient client = _httpClientFactory!.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.ImageDownloadTimeoutSeconds);
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

    private void OnBgTimerTick(object? sender, object e)
    {
        if (s_backgroundList.Count > 0)
        {
            ShowBackground((s_currentBgIndex + 1) % s_backgroundList.Count);
        }
    }
}
