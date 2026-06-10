//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.HoyoPlay.Connect.Package;

internal sealed class Package
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;

    [JsonPropertyName("game_pkgs")]
    public List<PackageSegment> GamePackages { get; set; } = default!;

    [JsonPropertyName("audio_pkgs")]
    public List<AudioPackageSegment> AudioPackages { get; set; } = default!;

    [JsonPropertyName("res_list_url")]
    public string ResourceListUrl { get; set; } = default!;

    [JsonIgnore]
    public List<PackageSegment> AllPackages
    {
        get => [.. GamePackages, .. AudioPackages];
    }
}
