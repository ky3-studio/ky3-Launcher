//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInRewardInfo
{
    [JsonPropertyName("total_sign_day")]
    public int TotalSignDay { get; set; }

    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    [JsonPropertyName("today")]
    public string? Today { get; set; }

    [JsonPropertyName("is_sign")]
    public bool IsSign { get; set; }

    [JsonPropertyName("is_sub")]
    public bool IsSub { get; set; }

    public string Region { get; set; } = default!;

    [JsonPropertyName("sign_cnt_missed")]
    public int SignCountMissed { get; set; }

    [JsonPropertyName("short_sign_day")]
    public int ShortSignDay { get; set; }
}