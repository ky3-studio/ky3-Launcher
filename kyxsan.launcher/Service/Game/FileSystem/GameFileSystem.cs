//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.Package.Advanced;

namespace kyxsan.Service.Game.FileSystem;

internal sealed partial class GameFileSystem : IGameFileSystem
{
    private readonly AsyncReaderWriterLock.Releaser releaser;

    private GameFileSystem(string gameFilePath, AsyncReaderWriterLock.Releaser releaser)
    {
        GameFilePath = gameFilePath;
        this.releaser = releaser;
    }

    public string GameFilePath { get; }

    [field: MaybeNull]
    public GameAudioInstallation Audio { get => field ??= new(this.GameDirectory); }

    public bool IsDisposed { get; private set; }

    public static IGameFileSystem Create(string gameFilePath, AsyncReaderWriterLock.Releaser releaser)
    {
        return new GameFileSystem(gameFilePath, releaser);
    }

    public static IGameFileSystem CreateForPackageOperation(string gameFilePath, GameAudioInstallation? gameAudioSystem = default)
    {
        return new PackageOperationGameFileSystem(gameFilePath, gameAudioSystem);
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        releaser.Dispose();
        IsDisposed = true;
    }
}