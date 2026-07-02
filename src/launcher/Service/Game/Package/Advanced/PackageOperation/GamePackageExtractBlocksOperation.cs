//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.IO;
using Launcher.Service.Game.FileSystem;
using Launcher.Service.Game.Package.Advanced.Model;
using Launcher.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;
using System.IO;

namespace Launcher.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.ExtractBlocks)]
internal sealed class GamePackageExtractBlocksOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild? localBuild = context.Operation.LocalBuild;
        ArgumentNullException.ThrowIfNull(localBuild);
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long downloadTotalBytes = context.Information.DownloadTotalBytes;
        long installTotalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));
        ImmutableArray<SophonAssetOperation> targetAssets = [.. diffAssets.Where(ao => ao.Kind is SophonAssetOperationKind.Modify)];
        ImmutableArray<string> targetAssetNames = [.. targetAssets.Select(ao => Path.GetFileName(ao.OldAsset.AssetName))];

        context.Progress.Report(new GamePackageOperationReport.Reset("Copying", 0, targetAssets.Length, targetAssets.Sum(sao => sao.OldAsset.AssetSize)));

        // We can just use the legacy chunk diffs to copy the required old blocks files because the files needed to patch are the same
        string oldBlksDirectory = Path.Combine(context.Operation.GameFileSystem.DataDirectory, @"StreamingAssets\AssetBundles\blocks");
        foreach (string file in Directory.GetFiles(oldBlksDirectory, "*.blk", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            if (!targetAssetNames.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            string newFilePath = Path.Combine(context.Operation.EffectiveGameDirectory, fileName);
            FileOperation.Copy(file, newFilePath, true);
            AssetProperty asset = localBuild.Manifests.Single().Data.Assets.Single(a => a.AssetName.Contains(fileName, StringComparison.OrdinalIgnoreCase));
            context.Progress.Report(new GamePackageOperationReport.Install(asset.AssetSize, asset.AssetChunks.Count));
        }

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", downloadTotalChunks, installTotalChunks, downloadTotalBytes, installTotalBytes));
        if (context.Operation.PatchBuild is { } patchBuild)
        {
            await context.Operation.Asset.InstallOrPatchAssetsAsync(context, patchBuild).ConfigureAwait(false);
        }
        else
        {
            await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);
        }

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }
}
