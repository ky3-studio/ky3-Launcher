//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.Package.Advanced.Model;
using System.Collections.Immutable;

namespace kyxsan.Service.Game.Package.Advanced;

internal sealed class GamePackageOperationInfo
{
    public GamePackageOperationInfo(int downloadTotalChunks, int installTotalChunks, long downloadTotalBytes, long installTotalBytes, ImmutableArray<SophonAssetOperation> diffAssetOperations)
    {
        DownloadTotalChunks = downloadTotalChunks;
        InstallTotalChunks = installTotalChunks;
        DownloadTotalBytes = downloadTotalBytes;
        InstallTotalBytes = installTotalBytes;
        DiffAssetOperations = diffAssetOperations;
    }

    public int DownloadTotalChunks { get; }

    public int InstallTotalChunks { get; }

    public long DownloadTotalBytes { get; }

    public long InstallTotalBytes { get; }

    public ImmutableArray<SophonAssetOperation> DiffAssetOperations { get; }
}