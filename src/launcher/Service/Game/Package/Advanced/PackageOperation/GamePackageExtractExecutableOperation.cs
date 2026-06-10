//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.Package.Advanced.Model;

namespace kyxsan.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.ExtractExecutable)]
internal sealed class GamePackageExtractExecutableOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild? remoteBuild = context.Operation.RemoteBuild;
        ArgumentNullException.ThrowIfNull(remoteBuild);
        int totalChunks = remoteBuild.TotalChunks;
        long downloadTotalBytes = remoteBuild.DownloadTotalBytes;
        long installTotalBytes = remoteBuild.UncompressedTotalBytes;

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.Single().Data.Assets.SelectMany(a => a.AssetChunks));

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", totalChunks, downloadTotalBytes, installTotalBytes));
        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }
}