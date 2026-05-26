//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.Configuration;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Package.Advanced.Model;
using System.Collections.Immutable;
using System.IO;

namespace kyxsan.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.Update)]
internal sealed class GamePackageUpdateOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild? remoteBuild = context.Operation.RemoteBuild;
        ArgumentNullException.ThrowIfNull(remoteBuild);

        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long downloadTotalBytes = context.Information.DownloadTotalBytes;
        long installTotalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedUpdating, downloadTotalChunks, installTotalChunks, downloadTotalBytes, installTotalBytes));

        if (context.Operation.PatchBuild is { } patchBuild)
        {
            await context.Operation.Asset.InstallOrPatchAssetsAsync(context, patchBuild).ConfigureAwait(false);
            await context.Operation.Asset.DeletePatchDeprecatedFilesAsync(context, patchBuild).ConfigureAwait(false);
        }
        else
        {
            await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);
        }

        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await PrivateVerifyAndRepairAsync(context, remoteBuild, remoteBuild.UncompressedTotalBytes, remoteBuild.TotalChunks).ConfigureAwait(false);

        GameConfiguration.UpdateVersion(context.Operation.GameFileSystem.GameConfigurationFilePath, remoteBuild.Tag);

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }
    }
}