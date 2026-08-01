//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Logging;
using Launcher.Service.Constants;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Net.Http;

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
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DownloadTimeoutSeconds);
            string response = await client.GetStringAsync(
                LauncherApiConstants.GameInfoApi);

            try
            {
                Directory.CreateDirectory(BgCacheDir);
                await File.WriteAllTextAsync(GetApiCachePath(), response);
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

            DispatcherQueue.TryEnqueue(async () =>
            {
                if (s_backgroundList.Count > 0)
                {
                    if (!shownFromCache || backgroundChanged)
                    {
                        await ShowBackgroundAsync(0);
                    }

                    LoadBgIndicators();
                    StartBgAutoSwitch();
                    PreloadAndCacheBackgroundImages();
                }
            });
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            if (ex is not (TaskCanceledException or HttpRequestException or OperationCanceledException))
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }

    private static List<BackgroundInfo> ParseBackgroundList(string apiResponse, string? targetBiz = null)
    {
        string expectedBiz = targetBiz ?? LauncherApiConstants.GameContentBiz;
        List<BackgroundInfo> list = [];
        try
        {
            using JsonDocument json = JsonDocument.Parse(apiResponse);
            JsonElement gameList = json.RootElement.GetProperty("data").GetProperty("game_info_list");

            foreach (JsonElement game in gameList.EnumerateArray())
            {
                string? biz = game.GetProperty("game").GetProperty("biz").GetString();
                if (biz != expectedBiz)
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
                    string firstCachePath = GetBgImageCachePath(s_backgroundList[0].ImageUrl);
                    if (File.Exists(firstCachePath))
                    {
                        s_cachedFirstBitmap = new BitmapImage(new Uri(firstCachePath));
                    }
                }

                if (s_cachedFirstBitmap != null && _mainView != null && _isPageActive && !_suppressStaticImage)
                {
                    _mainView.LauncherBackgroundImage.Source = s_cachedFirstBitmap;
                    _mainView.LauncherBackgroundImage.Opacity = 1;
                }

                return s_cachedFirstBitmap != null;
            }

            string apiCachePath = GetApiCachePath();
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
}
