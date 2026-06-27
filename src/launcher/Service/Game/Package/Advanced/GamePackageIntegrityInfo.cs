//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Service.Game.Package.Advanced.Model;
using Launcher.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.Collections.Immutable;

namespace Launcher.Service.Game.Package.Advanced;

internal sealed class GamePackageIntegrityInfo
{
    public required ImmutableArray<SophonAssetOperation> ConflictedAssets { get; init; }

    public required bool ChannelSdkConflicted { get; init; }

    public bool NoConflict { get => ConflictedAssets is [] && !ChannelSdkConflicted; }

    public (int ChunkCount, long ByteCount) GetConflictedBlockCountAndByteCount(GameChannelSDK? sdk)
    {
        int conflictChunks = ConflictedAssets.Sum(a => a.NewAsset.AssetChunks.Count);
        long conflictBytes = ConflictedAssets.Sum(a => a.NewAsset.AssetSize);

        if (ChannelSdkConflicted)
        {
            ArgumentNullException.ThrowIfNull(sdk);
            conflictChunks++;
            conflictBytes += sdk.ChannelSdkPackage.Size;
        }

        return (conflictChunks, conflictBytes);
    }
}