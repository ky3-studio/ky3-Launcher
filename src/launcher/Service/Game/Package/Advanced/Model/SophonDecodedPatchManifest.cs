//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Launcher.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedPatchManifest
{
    public SophonDecodedPatchManifest(string originalTag, string tag, string urlPrefix, string urlSuffix, PatchManifest data)
    {
        OriginalTag = originalTag;
        Tag = tag;
        UrlPrefix = urlPrefix;
        UrlSuffix = urlSuffix;
        Data = data;
    }

    public string OriginalTag { get; }

    public string Tag { get; }

    public string UrlPrefix { get; }

    public string UrlSuffix { get; }

    public PatchManifest Data { get; }
}