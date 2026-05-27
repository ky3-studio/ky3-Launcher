//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Package.Advanced.Model;
using kyxsan.Web.Hoyolab.Downloader;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;

namespace kyxsan.Service.Game.Package.Advanced;

internal interface IGamePackageService
{
    ValueTask<bool> ExecuteOperationAsync(GamePackageOperationContext context);

    ValueTask CancelOperationAsync();

    ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(IGameFileSystemView gameFileSystem, BranchWrapper? branch, CancellationToken token = default);

    ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(IGameFileSystemView gameFileSystem, SophonBuild? build, CancellationToken token = default);

    ValueTask<SophonDecodedPatchBuild?> DecodeDiffManifestsAsync(IGameFileSystemView gameFileSystem, BranchWrapper? branch, CancellationToken token = default);

    ValueTask<SophonDecodedPatchBuild?> DecodeDiffManifestsAsync(IGameFileSystemView gameFileSystem, SophonPatchBuild? patchBuild, CancellationToken token = default);
}