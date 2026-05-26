//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO.Ini;
using kyxsan.Service.Game.Configuration;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Scheme;
using System.Collections.Immutable;
using System.IO;

namespace kyxsan.Service.Game.Package.Advanced;

internal sealed class GameInstallPersistence
{
    private const string InstallingName = "snap_kyxsan_installing";
    private readonly IGameFileSystem gameFileSystem;

    private GameInstallPersistence(IGameFileSystem gameFileSystem)
    {
        this.gameFileSystem = gameFileSystem;
    }

    public static bool TryAcquire(IGameFileSystem gameFileSystem, string version, LaunchScheme launchScheme, [NotNullWhen(true)] out GameInstallPersistence? locker)
    {
        string gameDirectory = gameFileSystem.GameDirectory;
        Directory.CreateDirectory(gameDirectory);

        // If the directory is not empty
        if (Directory.EnumerateFileSystemEntries(gameDirectory).Any())
        {
            // we need to make sure config file exists and has our installing mark
            // Otherwise, we should prevent installation from proceeding
            if (!File.Exists(gameFileSystem.GameConfigurationFilePath) || !GameConfiguration.Read(gameFileSystem, InstallingName))
            {
                locker = default;
                return false;
            }

            // If the mark exists, we can proceed
            locker = new(gameFileSystem);
            return true;
        }

        // Directory is empty, create config file with our installing mark
        GameConfiguration.Create(launchScheme, version, gameFileSystem.GameConfigurationFilePath);
        ImmutableArray<IniElement> elements = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigurationFilePath);
        IniSerializer.SerializeToFile(gameFileSystem.GameConfigurationFilePath, elements.Add(new IniParameter(InstallingName, string.Empty)));

        locker = new(gameFileSystem);
        return true;
    }

    public void Release()
    {
        ImmutableArray<IniElement> elements = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigurationFilePath);
        IniSerializer.SerializeToFile(gameFileSystem.GameConfigurationFilePath, elements.Remove(elements.Single(e => e is IniParameter { Key: InstallingName })));
    }
}