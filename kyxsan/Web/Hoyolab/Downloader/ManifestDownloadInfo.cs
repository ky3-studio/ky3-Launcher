//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Downloader;

internal sealed class ManifestDownloadInfo
{
    [JsonPropertyName("encryption")]
    public uint Encryption { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;

    [JsonPropertyName("compression")]
    public uint Compression { get; set; }

    [JsonPropertyName("url_prefix")]
    public string UrlPrefix { get; set; } = default!;

    [JsonPropertyName("url_suffix")]
    public string UrlSuffix { get; set; } = default!;
}
