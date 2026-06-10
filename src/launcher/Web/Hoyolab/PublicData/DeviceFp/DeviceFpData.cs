//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.PublicData.DeviceFp;

internal sealed class DeviceFpData
{
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = default!;

    [JsonPropertyName("bbs_device_id")]
    public string? BbsDeviceId { get; set; }

    [JsonPropertyName("seed_id")]
    public string SeedId { get; set; } = default!;

    [JsonPropertyName("seed_time")]
    public string SeedTime { get; set; } = default!;

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = default!;

    [JsonPropertyName("device_fp")]
    public string DeviceFp { get; set; } = default!;

    [JsonPropertyName("app_name")]
    public string AppName { get; set; } = default!;

    [JsonPropertyName("ext_fields")]
    public string ExtFields { get; set; } = default!;
}