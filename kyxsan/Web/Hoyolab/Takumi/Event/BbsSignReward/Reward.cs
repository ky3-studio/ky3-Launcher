//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class Reward
{
    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("awards")]
    public ImmutableArray<Award> Awards { get; set; } = default!;

    [JsonPropertyName("biz")]
    public string Biz { get; set; } = default!;

    [JsonPropertyName("resign")]
    public bool Resign { get; set; }

    [JsonPropertyName("short_extra_award")]
    public ShortExtraAward ShortExtraAward { get; set; } = default!;
}