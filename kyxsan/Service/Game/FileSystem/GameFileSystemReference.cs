//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Service.Game.FileSystem;

internal sealed partial class GameFileSystemReference : IGameFileSystem
{
    private IGameFileSystem reference;

    public GameFileSystemReference(IGameFileSystem reference)
    {
        this.reference = reference;
    }

    public bool IsDisposed { get => reference.IsDisposed; }

    public string GameFilePath { get => reference.GameFilePath; }

    public GameAudioInstallation Audio { get => reference.Audio; }

    public void Exchange(IGameFileSystem newReference)
    {
        if (newReference is GameFileSystemReference wrapper)
        {
            Exchange(wrapper.reference);
            return;
        }

        if (!ReferenceEquals(reference, newReference))
        {
            reference.Dispose();
            reference = newReference;
        }
    }

    public void Dispose()
    {
        reference.Dispose();
    }
}