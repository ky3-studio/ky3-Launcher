//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Service.Game.FileSystem;

namespace Launcher.Service.Game.Package.Advanced;

internal sealed partial class PackageOperationGameFileSystem : IGameFileSystem
{
    public PackageOperationGameFileSystem(string gameFilePath, GameAudioInstallation? gameAudioSystem = default)
    {
        GameFilePath = gameFilePath;
        Audio = gameAudioSystem ?? new(this.GameDirectory);
    }

    public string GameFilePath { get; }

    public GameAudioInstallation Audio { get; }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
    }
}
