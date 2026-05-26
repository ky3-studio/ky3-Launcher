//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using kyxsan.Core;
using kyxsan.Core.LifeCycle;
using kyxsan.Core.Logging;
using kyxsan.Core.Setting;
using kyxsan.Core.Shell;
using kyxsan.UI.Windowing;
using kyxsan.UI.Xaml.View.Window;
using kyxsan.ViewModel.Guide;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace kyxsan.ViewModel;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class NotifyIconViewModel : ObservableObject
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly App app;
    private FlyoutBase? notifyIconContextMenu;
    private FrameworkElement? notifyIconContextMenuRoot;

    [GeneratedConstructor]
    public partial NotifyIconViewModel(IServiceProvider serviceProvider);

    public static string Title
    {
        get
        {
            string? title = kyxsanRuntime.GetDisplayName();
            ArgumentException.ThrowIfNullOrEmpty(title);
            return title;
        }
    }

    public partial RuntimeOptions RuntimeOptions { get; }

    [Command("CloseNotifyIconContextMenuWindowCommand")]
    private Task CloseNotifyIconContextMenuWindowAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Close notify icon context menu", "NotifyIconViewModel.Command"));
        return CloseNotifyIconContextMenuWithAnimationAsync();
    }

    internal void NotifyIconContextMenuClosed()
    {
        if (notifyIconContextMenuRoot is not null)
        {
            notifyIconContextMenuRoot.Opacity = 1;
            if (notifyIconContextMenuRoot.RenderTransform is ScaleTransform st)
            {
                st.ScaleX = 1;
                st.ScaleY = 1;
            }
        }
    }

    private async Task CloseNotifyIconContextMenuWithAnimationAsync()
    {
        if (notifyIconContextMenu is null)
        {
            return;
        }

        if (notifyIconContextMenuRoot is null)
        {
            notifyIconContextMenu.Hide();
            return;
        }

        FrameworkElement root = notifyIconContextMenuRoot;
        try
        {
            root.RenderTransformOrigin = new(0.5, 0.5);
            root.RenderTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };

            Storyboard storyboard = new();

            DoubleAnimation opacityAnimation = new()
            {
                To = 0,
                Duration = new(TimeSpan.FromMilliseconds(120)),
                EnableDependentAnimation = true,
            };

            DoubleAnimation scaleXAnimation = new()
            {
                To = 0.95,
                Duration = new(TimeSpan.FromMilliseconds(120)),
                EnableDependentAnimation = true,
            };

            DoubleAnimation scaleYAnimation = new()
            {
                To = 0.95,
                Duration = new(TimeSpan.FromMilliseconds(120)),
                EnableDependentAnimation = true,
            };

            Storyboard.SetTarget(opacityAnimation, root);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            Storyboard.SetTarget(scaleXAnimation, root);
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");

            Storyboard.SetTarget(scaleYAnimation, root);
            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");

            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);

            TaskCompletionSource tcs = new();
            void OnCompleted(object? s, object e)
            {
                storyboard.Completed -= OnCompleted;
                tcs.TrySetResult();
            }

            storyboard.Completed += OnCompleted;
            storyboard.Begin();

            await tcs.Task.ConfigureAwait(true);
        }
        catch
        {
            // Ignore animation failures, always close the flyout.
        }
        finally
        {
            notifyIconContextMenu.Hide();
            if (notifyIconContextMenuRoot is not null)
            {
                notifyIconContextMenuRoot.Opacity = 1;
                if (notifyIconContextMenuRoot.RenderTransform is ScaleTransform st)
                {
                    st.ScaleX = 1;
                    st.ScaleY = 1;
                }
            }
        }
    }

    internal void SetNotifyIconContextMenu(FlyoutBase flyout, FrameworkElement root)
    {
        notifyIconContextMenu = flyout;
        notifyIconContextMenuRoot = root;
    }

    [Command("RestartAsElevatedCommand")]
    private static void RestartAsElevated()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Restart as elevated", "NotifyIconViewModel.Command"));
        NativeMethods.RestartAsAdministrator();
    }

    [Command("ShowWindowCommand")]
    private void ShowWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Show window", "NotifyIconViewModel.Command"));

        switch (currentXamlWindowReference.Window)
        {
            case MainWindow mainWindow:
                {
                    if (mainWindow.AppWindow is not null)
                    {
                        mainWindow.SwitchTo();
                        mainWindow.AppWindow.MoveInZOrderAtTop();
                    }

                    return;
                }

            case null:
                {
                    GuideState guideState = UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language);
                    if (guideState < GuideState.Completed)
                    {
                        GuideWindow guideWindow = serviceProvider.GetRequiredService<GuideWindow>();
                        currentXamlWindowReference.Window = guideWindow;
                        guideWindow.SwitchTo();
                        guideWindow.AppWindow.MoveInZOrderAtTop();
                    }
                    else
                    {
                        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                        currentXamlWindowReference.Window = mainWindow;
                        mainWindow.SwitchTo();
                        mainWindow.AppWindow.MoveInZOrderAtTop();
                    }

                    return;
                }

            default:
                {
                    Window otherWindow = currentXamlWindowReference.Window;
                    otherWindow.SwitchTo();
                    otherWindow.AppWindow.MoveInZOrderAtTop();
                    return;
                }
        }
    }

    [Command("LaunchGameCommand")]
    private async Task LaunchGame()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch Game", "NotifyIconViewModel.Command"));
        if (serviceProvider.GetRequiredService<IAppActivation>() is IAppActivationActionHandlersAccess access)
        {
            await access.HandleLaunchGameActionAsync().ConfigureAwait(false);
        }
    }

    [Command("ExitCommand")]
    private void Exit()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Exit application", "NotifyIconViewModel.Command"));
        app.Exit();
    }

    [Command("TakeScreenshotCommand")]
    private async Task TakeScreenshotAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Take Window screenshot", "NotifyIconViewModel.Command"));

        if (currentXamlWindowReference.Window is null)
        {
            return;
        }

        Microsoft.UI.Xaml.Media.Imaging.RenderTargetBitmap renderTargetBitmap = new();
        await renderTargetBitmap.RenderAsync(currentXamlWindowReference.Window.Content);

        IBuffer pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
        int width = renderTargetBitmap.PixelWidth;
        int height = renderTargetBitmap.PixelHeight;

        string directory = Path.Combine(kyxsanRuntime.GetDataScreenshotDirectory(), CultureInfo.CurrentCulture.Name);
        Directory.CreateDirectory(directory);
        string filename = $"Screenshot_{DateTimeOffset.Now:yyyy.MM.dd_HH.mm.ss}.png";
        using (FileStream fileStream = File.Create(Path.Combine(directory, filename)))
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream.AsRandomAccessStream());
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)width, (uint)height, 72, 72, pixelBuffer.ToArray());
            await encoder.FlushAsync();
        }
    }

    public XamlRoot? XamlRoot { get; set; }
}

internal sealed partial class NotifyIconViewModel
{
    public static bool CanTakeScreenshot
    {
        get =>
#if DEBUG || IS_ALPHA_BUILD
            true;
#else
            false;
#endif
    }
}