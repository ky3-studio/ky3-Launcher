//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;

namespace kyxsan.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedBuild
{
    private static readonly Func<AssetProperty, int> SumAssetPropertyAssetChunks = property => property.AssetChunks.Count;
    private static readonly Func<SophonDecodedManifest, int> SumSophonDecodedManifestAssets = manifest => manifest.Data.Assets.Sum(SumAssetPropertyAssetChunks);

    public SophonDecodedBuild(string tag, long downloadTotalBytes, long uncompressedTotalBytes, ImmutableArray<SophonDecodedManifest> manifests)
    {
        Tag = tag;
        DownloadTotalBytes = downloadTotalBytes;
        UncompressedTotalBytes = uncompressedTotalBytes;
        Manifests = manifests;
    }

    public string Tag { get; }

    public long DownloadTotalBytes { get; }

    public long UncompressedTotalBytes { get; }

    public ImmutableArray<SophonDecodedManifest> Manifests { get; }

    public int TotalChunks { get => Manifests.Sum(SumSophonDecodedManifestAssets); }
}