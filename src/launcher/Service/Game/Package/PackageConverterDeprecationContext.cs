//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.FileSystem;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using System.Net.Http;

namespace kyxsan.Service.Game.Package;

internal readonly struct PackageConverterDeprecationContext
{
    public readonly HttpClient HttpClient;
    public readonly IGameFileSystemView GameFileSystem;
    public readonly GameChannelSDK? GameChannelSdk;
    public readonly DeprecatedFilesWrapper? DeprecatedFiles;

    public PackageConverterDeprecationContext(HttpClient httpClient, IGameFileSystemView gameFileSystem, GameChannelSDK? gameChannelSdk, DeprecatedFilesWrapper? deprecatedFiles)
    {
        HttpClient = httpClient;
        GameFileSystem = gameFileSystem;
        GameChannelSdk = gameChannelSdk;
        DeprecatedFiles = deprecatedFiles;
    }
}