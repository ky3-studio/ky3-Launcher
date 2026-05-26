//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Setting;
using kyxsan.Win32;
using System.IO;

namespace kyxsan.Core.IO;

internal static class PhysicalDrive
{
    /// <summary>
    /// Safely get the SSD information of the physical driver.
    /// </summary>
    /// <param name="path">path in a driver</param>
    /// <returns>
    /// <see langword="null"/> if any exception occurs,
    /// <see langword="true"/> if it's a SSD,
    /// otherwise <see langword="false"/>
    /// </returns>
    public static bool? GetIsSolidState(string path)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? pathUri))
        {
            if (pathUri.IsUnc)
            {
                return false;
            }
        }

        try
        {
            return DangerousGetIsSolidState(path);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static bool DangerousGetIsSolidState(string path)
    {
        if (LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false))
        {
            return LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        }

        string? root = Path.GetPathRoot(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");
        return kyxsanNative.Instance.MakePhysicalDrive().IsPathOnSolidStateDrive(root);
    }
}