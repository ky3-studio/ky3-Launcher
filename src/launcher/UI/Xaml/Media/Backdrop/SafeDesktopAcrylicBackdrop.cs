//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _ | | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Collections.Concurrent;

namespace Launcher.UI.Xaml.Media.Backdrop;

internal sealed partial class SafeDesktopAcrylicBackdrop : SystemBackdrop
{
    private readonly ConcurrentDictionary<ICompositionSupportsSystemBackdrop, DesktopAcrylicController> controllers = [];
    private readonly ConcurrentDictionary<ICompositionSupportsSystemBackdrop, FrameworkElement> themeSources = [];

    private readonly DesktopAcrylicKind kind;

    public SafeDesktopAcrylicBackdrop(DesktopAcrylicKind kind = DesktopAcrylicKind.Default)
    {
        this.kind = kind;
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(target, xamlRoot);

        if (controllers.TryRemove(target, out DesktopAcrylicController? existing))
        {
            existing.RemoveSystemBackdropTarget(target);
            existing.Dispose();
        }

        if (themeSources.TryRemove(target, out FrameworkElement? staleSource))
        {
            staleSource.ActualThemeChanged -= OnActualThemeChanged;
        }

        try
        {
            SystemBackdropConfiguration configuration = GetDefaultSystemBackdropConfiguration(target, xamlRoot);
            DesktopAcrylicController newController = new() { Kind = kind };
            newController.AddSystemBackdropTarget(target);
            newController.SetSystemBackdropConfiguration(configuration);
            controllers.TryAdd(target, newController);

            if (xamlRoot.Content is FrameworkElement themeSource)
            {
                ApplyThemeTint(newController, themeSource.ActualTheme);
                themeSource.ActualThemeChanged += OnActualThemeChanged;
                themeSources.TryAdd(target, themeSource);
            }
        }
        catch (ArgumentException)
        {
        }
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        base.OnTargetDisconnected(target);

        if (themeSources.TryRemove(target, out FrameworkElement? themeSource))
        {
            themeSource.ActualThemeChanged -= OnActualThemeChanged;
        }

        if (controllers.TryRemove(target, out DesktopAcrylicController? controller))
        {
            controller.RemoveSystemBackdropTarget(target);
            controller.Dispose();
        }
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        foreach (KeyValuePair<ICompositionSupportsSystemBackdrop, FrameworkElement> pair in themeSources)
        {
            if (ReferenceEquals(pair.Value, sender) && controllers.TryGetValue(pair.Key, out DesktopAcrylicController? controller))
            {
                ApplyThemeTint(controller, sender.ActualTheme);
            }
        }
    }


    private void ApplyThemeTint(DesktopAcrylicController controller, ElementTheme theme)
    {
        if (kind is not DesktopAcrylicKind.Thin)
        {
            return;
        }

        try
        {
            if (theme is ElementTheme.Light)
            {
                controller.TintColor = Windows.UI.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                controller.TintOpacity = 0.72f;
                controller.LuminosityOpacity = 0.88f;
            }
            else
            {
                controller.TintColor = Windows.UI.Color.FromArgb(0xFF, 0x20, 0x20, 0x20);
                controller.TintOpacity = 0.62f;
                controller.LuminosityOpacity = 0.90f;
            }
        }
        catch (Exception)
        {
            // 不支持自定义材质参数的平台保持系统默认观感
        }
    }
}
