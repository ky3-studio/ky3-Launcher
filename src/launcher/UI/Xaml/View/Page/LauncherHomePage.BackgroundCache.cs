//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.IO;
using Launcher.Core.Logging;
using Launcher.Service.Constants;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LauncherHomePage
{
    private static string GetApiCachePath()
    {
        return Path.Combine(BgCacheDir, LauncherApiConstants.IsOverseaHomeApi ? "api_os.json" : "api_cn.json");
    }

    private static string GetBgImageCachePath(string imageUrl)
    {
        string ext = Path.GetExtension(new Uri(imageUrl).AbsolutePath);
        if (string.IsNullOrEmpty(ext))
        {
            ext = ".cache";
        }

        return Path.Combine(BgCacheDir, $"bg_{HashToHex(imageUrl)}{ext}");
    }

    private static string GetVideoCachePath(string videoUrl)
    {
        string ext = Path.GetExtension(new Uri(videoUrl).AbsolutePath);
        if (string.IsNullOrEmpty(ext)) ext = ".webm";
        return Path.Combine(BgCacheDir, $"bg_{HashToHex(videoUrl)}{ext}");
    }

    private static string HashToHex(string input)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash);
    }

    private async void PreloadAndCacheBackgroundImages()
    {
        try
        {
            Directory.CreateDirectory(BgCacheDir);

            BackgroundInfo[] snapshot = s_backgroundList.ToArray();

            List<Task> imageTasks = [];
            foreach (BackgroundInfo bg in snapshot)
            {
                imageTasks.Add(EnsureFileCachedAsync(bg.ImageUrl, GetBgImageCachePath(bg.ImageUrl)));
            }
            await Task.WhenAll(imageTasks);

            List<Task> themeTasks = [];
            foreach (BackgroundInfo bg in snapshot)
            {
                if (!string.IsNullOrEmpty(bg.ThemeUrl))
                {
                    themeTasks.Add(EnsureFileCachedAsync(bg.ThemeUrl, GetBgImageCachePath(bg.ThemeUrl)));
                }
            }
            await Task.WhenAll(themeTasks);

            List<Task> videoTasks = [];
            foreach (BackgroundInfo bg in snapshot)
            {
                if (!string.IsNullOrEmpty(bg.VideoUrl))
                {
                    videoTasks.Add(EnsureFileCachedAsync(bg.VideoUrl, GetVideoCachePath(bg.VideoUrl)));
                }
            }
            await Task.WhenAll(videoTasks);

            s_dataInitialized = true;

            CleanupStaleBgCache();
        }
        catch
        {
        }
    }

    private async Task EnsureFileCachedAsync(string url, string cachePath)
    {
        try
        {
            if (File.Exists(cachePath) && new FileInfo(cachePath).Length > 1024)
            {
                return;
            }

            using HttpClient client = _httpClientFactory!.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DownloadTimeoutSeconds);
            byte[] data = await client.GetByteArrayAsync(url);

            Directory.CreateDirectory(BgCacheDir);
            string tempPath = cachePath + ".tmp";
            await File.WriteAllBytesAsync(tempPath, data);
            File.Move(tempPath, cachePath, true);
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

            HashSet<string> validFiles = new(StringComparer.OrdinalIgnoreCase) { "api_cn.json", "api_os.json" };
            AddValidCacheFiles(validFiles, Path.Combine(BgCacheDir, "api_cn.json"), LauncherApiConstants.GameContentBizChinese);
            AddValidCacheFiles(validFiles, Path.Combine(BgCacheDir, "api_os.json"), LauncherApiConstants.GameContentBizOversea);
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
                if (!validFiles.Contains(fileName) &&
                    !fileName.StartsWith("content_", StringComparison.OrdinalIgnoreCase))
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

    private static void AddValidCacheFiles(HashSet<string> validFiles, string apiFilePath, string biz)
    {
        if (!File.Exists(apiFilePath))
        {
            return;
        }

        try
        {
            List<BackgroundInfo> cachedList = ParseBackgroundList(File.ReadAllText(apiFilePath), biz);
            foreach (BackgroundInfo bg in cachedList)
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
        }
        catch
        {
        }
    }
}
