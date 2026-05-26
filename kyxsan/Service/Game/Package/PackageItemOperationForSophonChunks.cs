//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.Package.Advanced.Model;
using kyxsan.Web.Hoyolab.Takumi.Downloader.Proto;

namespace kyxsan.Service.Game.Package;

internal sealed class PackageItemOperationForSophonChunks
{
    public PackageItemOperationKind Kind { get; init; }

    public string UrlPrefix { get; init; } = default!;

    public string UrlSuffix { get; init; } = default!;

    public AssetProperty OldAsset { get; init; } = default!;

    public AssetProperty NewAsset { get; init; } = default!;

    public List<SophonChunk> DiffChunks { get; init; } = [];

    public static PackageItemOperationForSophonChunks Add(string urlPrefix, string urlSuffix, AssetProperty newAsset)
    {
        return new()
        {
            Kind = PackageItemOperationKind.Add,
            UrlPrefix = string.Intern(urlPrefix),
            UrlSuffix = string.Intern(urlSuffix),
            NewAsset = newAsset,
            DiffChunks = newAsset.AssetChunks.Select(chunk => new SophonChunk(urlPrefix, urlSuffix, chunk)).ToList(),
        };
    }

    public static PackageItemOperationForSophonChunks ModifyOrReplace(string urlPrefix, string urlSuffix, AssetProperty oldAsset, AssetProperty newAsset, List<SophonChunk> diffChunks)
    {
        return new()
        {
            Kind = PackageItemOperationKind.Replace,
            UrlPrefix = string.Intern(urlPrefix),
            UrlSuffix = string.Intern(urlSuffix),
            OldAsset = oldAsset,
            NewAsset = newAsset,
            DiffChunks = diffChunks,
        };
    }

    public static PackageItemOperationForSophonChunks Backup(AssetProperty oldAsset)
    {
        return new()
        {
            Kind = PackageItemOperationKind.Backup,
            OldAsset = oldAsset,
        };
    }
}