//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class ExtraAwardInfo
{
    [JsonPropertyName("awards")]
    public List<ExtraAward> Awards { get; set; } = default!;

    [JsonPropertyName("total_cnt")]
    public int TotalCount { get; set; }

    [JsonPropertyName("ys_first_award")]
    public bool YsFirstAward { get; set; }

    [JsonPropertyName("has_short_act")]
    public bool HasShortAct { get; set; }

    [JsonPropertyName("short_act_info")]
    public ShortActInfo ShortActInfo { get; set; } = default!;
}