//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Core.IO;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using System.IO;

namespace kyxsan.Service.Game.Configuration;

[Service(ServiceLifetime.Singleton, typeof(IGameConfigurationFileService))]
internal sealed class GameConfigurationFileService : IGameConfigurationFileService
{
    private const string BackupChineseConfigurationFileName = "config_cn.ini";
    private const string BackupOverseaConfigurationFileName = "config_oversea.ini";

    public void Backup(string source, bool isOversea)
    {
        if (!File.Exists(source))
        {
            return;
        }

        string configFileName = isOversea ? BackupOverseaConfigurationFileName : BackupChineseConfigurationFileName;
        string destination = Path.Combine(kyxsanRuntime.GetDataServerCacheDirectory(), configFileName);
        FileOperation.Copy(source, destination, true);
    }

    public void Restore(string destination, bool isOversea)
    {
        string configFileName = isOversea ? BackupOverseaConfigurationFileName : BackupChineseConfigurationFileName;
        string source = Path.Combine(kyxsanRuntime.GetDataServerCacheDirectory(), configFileName);

        if (!File.Exists(source))
        {
            return;
        }

        // If target directory does not exist, do not copy the file
        // This often means user has moved the game folder away.
        string? directory = Path.GetDirectoryName(destination);
        if (!Directory.Exists(directory))
        {
            return;
        }

        try
        {
            FileOperation.Copy(source, destination, true);
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException ex)
        {
            if (kyxsanNative.IsWin32(ex.HResult, [WIN32_ERROR.ERROR_PATH_NOT_FOUND, WIN32_ERROR.ERROR_NO_SUCH_DEVICE]))
            {
                return;
            }

            throw;
        }
    }
}