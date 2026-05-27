//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab.Takumi.Downloader.Proto;

namespace kyxsan.Service.Game.Package.Advanced.Model;

internal sealed class SophonChunk
{
    public SophonChunk(string urlPrefix, string urlSuffix, AssetChunk assetChunk)
    {
        UrlPrefix = string.Intern(urlPrefix);
        UrlSuffix = string.Intern(urlSuffix);
        AssetChunk = assetChunk;
    }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public AssetChunk AssetChunk { get; }

    public string ChunkDownloadUrl { get => $"{UrlPrefix}/{AssetChunk.ChunkName}{UrlSuffix}"; }
}