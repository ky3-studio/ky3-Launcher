//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Launcher.Core.ExceptionService;

namespace Launcher.UI.Xaml.Control.Theme;

internal static class ThemeHelper
{
    public static SystemBackdropTheme ElementToSystemBackdrop(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => SystemBackdropTheme.Default,
            ElementTheme.Light => SystemBackdropTheme.Light,
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            _ => throw LauncherException.NotSupported($"Unexpected ElementTheme value: {elementTheme}."),
        };
    }

    public static Launcher.UI.Xaml.Theme ElementToFramework(ElementTheme elementTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => Launcher.UI.Xaml.Theme.None,
            ElementTheme.Light => Launcher.UI.Xaml.Theme.Light,
            ElementTheme.Dark => Launcher.UI.Xaml.Theme.Dark,
            _ => throw LauncherException.NotSupported($"Unexpected ElementTheme value: {elementTheme}."),
        };
    }

    public static Launcher.UI.Xaml.Theme ApplicationToFramework(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => Launcher.UI.Xaml.Theme.Light,
            ApplicationTheme.Dark => Launcher.UI.Xaml.Theme.Dark,
            _ => throw LauncherException.NotSupported($"Unexpected ApplicationTheme value: {applicationTheme}."),
        };
    }

    public static Launcher.UI.Xaml.Theme ApplicationToFrameworkInvert(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => Launcher.UI.Xaml.Theme.Dark,
            ApplicationTheme.Dark => Launcher.UI.Xaml.Theme.Light,
            _ => throw LauncherException.NotSupported($"Unexpected ApplicationTheme value: {applicationTheme}."),
        };
    }

    public static ApplicationTheme ElementToApplication(ElementTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ElementTheme.Light => ApplicationTheme.Light,
            ElementTheme.Dark => ApplicationTheme.Dark,
            _ => Application.Current.RequestedTheme,
        };
    }

    public static ElementTheme ApplicationToElement(ApplicationTheme applicationTheme)
    {
        return applicationTheme switch
        {
            ApplicationTheme.Light => ElementTheme.Light,
            ApplicationTheme.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }

    public static bool IsDarkMode(ApplicationTheme applicationTheme)
    {
        return applicationTheme is ApplicationTheme.Dark;
    }

    public static bool IsDarkMode(ElementTheme elementTheme)
    {
        ApplicationTheme appTheme = Application.Current.RequestedTheme;
        return IsDarkMode(elementTheme, appTheme);
    }

    public static bool IsDarkMode(ElementTheme elementTheme, ApplicationTheme applicationTheme)
    {
        return elementTheme switch
        {
            ElementTheme.Default => IsDarkMode(applicationTheme),
            ElementTheme.Dark => true,
            _ => false,
        };
    }
}
