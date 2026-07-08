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
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Composition;
using Microsoft.Web.WebView2.Core;
using Launcher.Core.IO;
using Launcher.Core.Logging;
using Launcher.UI.Content;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Game;
using Launcher.Service.Game.Scheme;
using Launcher.Service.Game.Package;
using Launcher.Service.User;
using Launcher.Service.Constants;
using Launcher.Model.Entity;
using Launcher.ViewModel.User;
using Launcher.ViewModel.Game;
using Launcher.Web.WebView2;
using System.IO;
using System.Net.Http;
using System.Numerics;
using Windows.System;
using Windows.UI;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LauncherHomePage
{
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
            const float peakBlur = 12f;
            TimeSpan phaseDuration = TimeSpan.FromMilliseconds(280);
            CompositionEasingFunction ease = compositor.CreateCubicBezierEasingFunction(
                new Vector2(0.4f, 0f), new Vector2(0.2f, 1f));

            ScalarKeyFrameAnimation blurUp = compositor.CreateScalarKeyFrameAnimation();
            blurUp.InsertKeyFrame(0f, 0f);
            blurUp.InsertKeyFrame(1f, peakBlur, ease);
            blurUp.Duration = phaseDuration;

            CompositionScopedBatch phase1 = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _blurBrush.StartAnimation("Blur.BlurAmount", blurUp);
            phase1.End();

            phase1.Completed += (_, _) =>
            {
                DispatcherQueue?.TryEnqueue(() =>
                {
                    if (_mainView == null || _blurVisual == null || _blurBrush == null)
                    {
                        return;
                    }

                    _mainView.LauncherBackgroundVideo.Opacity = 1;

                    ScalarKeyFrameAnimation blurDown = _blurVisual.Compositor.CreateScalarKeyFrameAnimation();
                    blurDown.InsertKeyFrame(0f, peakBlur);
                    blurDown.InsertKeyFrame(1f, 0f, ease);
                    blurDown.Duration = phaseDuration;
                    _blurBrush.StartAnimation("Blur.BlurAmount", blurDown);
                });
            };
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
            const float peakBlur = 12f;
            TimeSpan phaseDuration = TimeSpan.FromMilliseconds(280);
            CompositionEasingFunction ease = compositor.CreateCubicBezierEasingFunction(
                new Vector2(0.4f, 0f), new Vector2(0.2f, 1f));

            ScalarKeyFrameAnimation blurUp = compositor.CreateScalarKeyFrameAnimation();
            blurUp.InsertKeyFrame(0f, 0f);
            blurUp.InsertKeyFrame(1f, peakBlur, ease);
            blurUp.Duration = phaseDuration;

            CompositionScopedBatch phase1 = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _blurBrush.StartAnimation("Blur.BlurAmount", blurUp);
            phase1.End();

            phase1.Completed += (_, _) =>
            {
                DispatcherQueue?.TryEnqueue(() =>
                {
                    if (_mainView == null || _blurVisual == null || _blurBrush == null)
                    {
                        onCompleted?.Invoke();
                        return;
                    }

                    _mainView.LauncherBackgroundVideo.Opacity = 0;
                    _mainView.LauncherBackgroundTheme.Opacity = 0;

                    ScalarKeyFrameAnimation blurDown = _blurVisual.Compositor.CreateScalarKeyFrameAnimation();
                    blurDown.InsertKeyFrame(0f, peakBlur);
                    blurDown.InsertKeyFrame(1f, 0f, ease);
                    blurDown.Duration = phaseDuration;

                    CompositionScopedBatch phase2 = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    _blurBrush.StartAnimation("Blur.BlurAmount", blurDown);
                    phase2.End();

                    phase2.Completed += (_, _) =>
                    {
                        DispatcherQueue?.TryEnqueue(() => onCompleted?.Invoke());
                    };
                });
            };
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

    private async void PlayCurrentVideo()
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
            await EnsureVideoWebView2Async();
            if (_mainView == null || !_videoWebView2Ready || !_isPageActive || !s_isVideoMode)
            {
                return;
            }

            string videoSrc;
            string localPath = GetVideoCachePath(bg.VideoUrl);
            if (File.Exists(localPath))
            {
                videoSrc = "https://bgvideo.local/" + Path.GetFileName(localPath);
            }
            else
            {
                string oldCache = GetBgImageCachePath(bg.VideoUrl);
                if (File.Exists(oldCache))
                {
                    FileOperationSafe.TryMove(oldCache, localPath);
                }

                videoSrc = File.Exists(localPath)
                    ? "https://bgvideo.local/" + Path.GetFileName(localPath)
                    : bg.VideoUrl;
            }

            string videoHtml = BuildVideoHtml(videoSrc);
            _mainView.LauncherBackgroundVideo.CoreWebView2.NavigateToString(videoHtml);

            _ = UpdateThemeOverlay(s_currentBgIndex);
            AnimateVideoIn();
        }
        catch (Exception ex)
        {
            StopVideo();
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "Video playback failed", "LauncherHomePage",
                [("Error", ex.Message)]));
        }
    }

    private async Task EnsureVideoWebView2Async()
    {
        if (_videoWebView2Ready || _mainView == null)
        {
            return;
        }

        try
        {
            CoreWebView2Environment env = await CoreWebView2EnvironmentFactory.GetAsync();
            await _mainView.LauncherBackgroundVideo.EnsureCoreWebView2Async(env);

            _mainView.LauncherBackgroundVideo.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "bgvideo.local", BgCacheDir, CoreWebView2HostResourceAccessKind.Allow);

            CoreWebView2Settings settings = _mainView.LauncherBackgroundVideo.CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = false;
            settings.AreDevToolsEnabled = false;
            settings.IsStatusBarEnabled = false;
            settings.IsZoomControlEnabled = false;
            settings.AreBrowserAcceleratorKeysEnabled = false;

            _videoWebView2Ready = true;
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "WebView2 initialization failed", "LauncherHomePage",
                [("Error", ex.Message)]));
        }
    }

    private static string BuildVideoHtml(string videoSrc)
    {
        return "<!DOCTYPE html><html><head><style>*{margin:0;padding:0;overflow:hidden;background:transparent}video{width:100vw;height:100vh;object-fit:cover}</style></head><body><video autoplay loop muted playsinline src=\"" + videoSrc + "\"></video></body></html>";
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
            using HttpClient client = _httpClientFactory!.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DownloadTimeoutSeconds);
            string response = await client.GetStringAsync(
                LauncherApiConstants.MiHoYoGameContentApi);

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
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "Content load failed", "LauncherHomePage",
                [("Error", ex.Message)]));
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
            _bannerTimer.Tick += OnBannerTimerTick;
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
                await Windows.System.Launcher.LaunchUriAsync(new Uri(link));
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
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(link));
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
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
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

    private void OnBannerTimerTick(object? sender, object e)
    {
        if (_bannerList.Count > 0)
        {
            ShowBanner((_currentBannerIndex + 1) % _bannerList.Count);
        }
    }
}
