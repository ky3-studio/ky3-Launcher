//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedPatchBuild
{
    public SophonDecodedPatchBuild(string originalTag, string tag, long downloadTotalBytes, long downloadFileCount, long uncompressedTotalBytes, long installFileCount, ImmutableArray<SophonDecodedPatchManifest> manifests)
    {
        OriginalTag = originalTag;
        Tag = tag;
        DownloadTotalBytes = downloadTotalBytes;
        DownloadFileCount = downloadFileCount;
        UncompressedTotalBytes = uncompressedTotalBytes;
        InstallFileCount = installFileCount;
        Manifests = manifests;
    }

    public string OriginalTag { get; }

    public string Tag { get; }

    public long DownloadTotalBytes { get; }

    public long DownloadFileCount { get; }

    public long UncompressedTotalBytes { get; }

    public long InstallFileCount { get; }

    public ImmutableArray<SophonDecodedPatchManifest> Manifests { get; }
}