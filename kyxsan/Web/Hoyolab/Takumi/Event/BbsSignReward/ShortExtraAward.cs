//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class ShortExtraAward
{
    [JsonPropertyName("has_extra_award")]
    public bool HasExtraAward { get; set; }

    [JsonPropertyName("start_time")]
    public string StartTime { get; set; } = default!;

    [JsonPropertyName("end_time")]
    public string EndTime { get; set; } = default!;

    [JsonPropertyName("list")]
    public List<JsonElement> List { get; set; } = default!;

    [JsonPropertyName("start_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long StartTimestamp { get; set; } = default!;

    [JsonPropertyName("end_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long EndTimestamp { get; set; } = default!;
}