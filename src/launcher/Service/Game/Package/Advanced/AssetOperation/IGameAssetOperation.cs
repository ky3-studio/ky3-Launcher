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

namespace kyxsan.Service.Game.Package.Advanced.AssetOperation;

internal interface IGameAssetOperation
{
    ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild);

    ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(GamePackageServiceContext context, SophonDecodedBuild build);

    ValueTask RepairGamePackageAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info);

    ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets);

    ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets);

    ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context);

    ValueTask InstallOrPatchAssetsAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    ValueTask DeletePatchDeprecatedFilesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    ValueTask PredownloadPatchesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);
}