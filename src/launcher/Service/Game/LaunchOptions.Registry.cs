//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

namespace Launcher.Service.Game;

internal sealed partial class LaunchOptions
{
    internal bool IsRefreshingFromRegistry { get; private set; }

    public void LoadGameResolutionFromConfig(string gameDirectory)
    {
        RefreshFromRegistry();
    }

    public void RefreshFromRegistry()
    {
        if (IsRefreshingFromRegistry)
        {
            return;
        }

        IsRefreshingFromRegistry = true;
        try
        {
            string[] registryPaths =
            [
                @"SOFTWARE\miHoYo\原神",
                @"SOFTWARE\miHoYo\Genshin Impact",
                @"SOFTWARE\WOW6432Node\miHoYo\原神",
                @"SOFTWARE\WOW6432Node\miHoYo\Genshin Impact",
                @"Software\miHoYo\原神",
                @"Software\miHoYo\Genshin Impact",
            ];

            foreach (string path in registryPaths)
            {
                using Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
                if (key is null)
                {
                    continue;
                }

                string? widthName = null;
                string? heightName = null;

                foreach (string name in key.GetValueNames())
                {
                    if (name.Contains("Width") && !name.Contains("Graphics"))
                    {
                        widthName = name;
                    }
                    else if (name.Contains("Height") && !name.Contains("Graphics"))
                    {
                        heightName = name;
                    }
                }

                if (widthName is null || heightName is null)
                {
                    continue;
                }

                object? widthObj = key.GetValue(widthName);
                object? heightObj = key.GetValue(heightName);

                if (widthObj is int width && heightObj is int height && width > 0 && height > 0 && width < 10000 && height < 10000)
                {
                    ScreenWidth.Value = width;
                    ScreenHeight.Value = height;

                    int matchedIndex = FindMatchingResolutionIndex(width, height);
                    ResolutionPresetIndex.Value = matchedIndex;
                }

                return;
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
        finally
        {
            IsRefreshingFromRegistry = false;
        }
    }

    public void WriteDisplaySettingsToRegistry(bool isOversea)
    {
        ClampResolution();

        try
        {
            string[] registryPaths = isOversea
                ? [
                    @"Software\miHoYo\Genshin Impact",
                    @"SOFTWARE\miHoYo\Genshin Impact",
                    @"SOFTWARE\WOW6432Node\miHoYo\Genshin Impact",
                  ]
                : [
                    @"Software\miHoYo\原神",
                    @"SOFTWARE\miHoYo\原神",
                    @"SOFTWARE\WOW6432Node\miHoYo\原神",
                  ];

            foreach (string path in registryPaths)
            {
                using Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, writable: true);
                if (key is null)
                {
                    continue;
                }

                foreach (string name in key.GetValueNames())
                {
                    if (name.Contains("Fullscreen"))
                    {
                        int mode = IsFullScreen.Value ? 1 : (IsBorderless.Value ? 2 : 0);
                        key.SetValue(name, mode, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                    else if (name.Contains("Width") && !name.Contains("Graphics") && IsScreenWidthEnabled.Value)
                    {
                        key.SetValue(name, ScreenWidth.Value, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                    else if (name.Contains("Height") && !name.Contains("Graphics") && IsScreenHeightEnabled.Value)
                    {
                        key.SetValue(name, ScreenHeight.Value, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                }

                return;
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
}
