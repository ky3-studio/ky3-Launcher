//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Service.Constants;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System.Net.Http;
using System.Numerics;
using Windows.Storage.Streams;
using Windows.UI;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LauncherHomePage
{
    private async void ShowBackground(int index, bool forceNoAnimation = false)
    {
        await ShowBackgroundAsync(index, forceNoAnimation);
    }

    private async Task ShowBackgroundAsync(int index, bool forceNoAnimation = false)
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
        if (index < 0 || index >= s_backgroundList.Count)
        {
            return null;
        }

        string diskPath = GetBgImageCachePath(s_backgroundList[index].ImageUrl);
        if (!File.Exists(diskPath))
        {
            await EnsureFileCachedAsync(s_backgroundList[index].ImageUrl, diskPath);
        }

        if (!File.Exists(diskPath))
        {
            return null;
        }

        try
        {
            return new BitmapImage { UriSource = new Uri(diskPath) };
        }
        catch
        {
            return null;
        }
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
            string diskPath = GetBgImageCachePath(themeUrl);
            if (!File.Exists(diskPath))
            {
                using HttpClient client = _httpClientFactory!.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.ImageDownloadTimeoutSeconds);
                byte[] data = await client.GetByteArrayAsync(themeUrl);
                Directory.CreateDirectory(BgCacheDir);
                await File.WriteAllBytesAsync(diskPath, data);
            }

            BitmapImage themeBmp = new(new Uri(diskPath));

            if (_mainView != null && s_currentBgIndex == index)
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
            bool active = i == s_currentBgIndex;

            Border ring = new()
            {
                Width = 22,
                Height = 22,
                CornerRadius = new CornerRadius(11),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = active ? 1 : 0,
            };

            Border dot = new()
            {
                Width = 12,
                Height = 12,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(active ? Colors.White : Color.FromArgb(150, 255, 255, 255)),
            };

            Grid container = new()
            {
                Width = 22,
                Height = 22,
                Margin = new Thickness(4, 0, 4, 0),
                Background = new SolidColorBrush(Colors.Transparent),
            };
            container.Children.Add(ring);
            container.Children.Add(dot);

            int index = i;
            container.PointerPressed += (_, _) => ShowBackground(index);
            BgIndicators.Children.Add(container);
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

    private void OnBgTimerTick(object? sender, object e)
    {
        int count = s_backgroundList.Count;
        if (count <= 1)
        {
            return;
        }

        for (int step = 1; step <= count; step++)
        {
            int next = (s_currentBgIndex + step) % count;
            if (next == s_currentBgIndex)
            {
                break;
            }

            if (File.Exists(GetBgImageCachePath(s_backgroundList[next].ImageUrl)))
            {
                ShowBackground(next);
                return;
            }
        }
    }
}
