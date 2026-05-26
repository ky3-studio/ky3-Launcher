//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO.Ini;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using System.IO;

namespace kyxsan.Service.Game.Configuration;

internal static class GameScriptVersion
{
    public static bool Patch(string configFilePath, string scriptVersionFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            return false;
        }

        try
        {
            string? version = default;
            foreach (IniElement element in IniSerializer.DeserializeFromFile(configFilePath))
            {
                if (element is IniParameter { Key: GameConstants.GameVersion } parameter)
                {
                    version = parameter.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(version))
            {
                return false;
            }

            string? directory = Path.GetDirectoryName(scriptVersionFilePath);
            ArgumentNullException.ThrowIfNull(directory);
            Directory.CreateDirectory(directory);

            File.WriteAllText(scriptVersionFilePath, version);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // Access to the path '.*?' is denied.
            return false;
        }
        catch (IOException ex)
        {
            if (kyxsanNative.IsWin32(ex.HResult, [WIN32_ERROR.ERROR_NO_SUCH_DEVICE, WIN32_ERROR.ERROR_DEVICE_HARDWARE_ERROR, WIN32_ERROR.ERROR_FILE_CORRUPT]))
            {
                return false;
            }

            throw;
        }
    }
}