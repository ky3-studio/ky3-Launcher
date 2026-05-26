//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInRewardReSignInfo
{
    [JsonPropertyName("resign_cnt_daily")]
    public uint ResignCountDaily { get; set; }

    [JsonPropertyName("resign_cnt_monthly")]
    public uint ResignCountMonthly { get; set; }

    [JsonPropertyName("resign_limit_daily")]
    public uint ResignLimitDaily { get; set; }

    [JsonPropertyName("resign_limit_monthly")]
    public uint ResignLimitMonthly { get; set; }

    [JsonPropertyName("sign_cnt_missed")]
    public uint SignCountMissed { get; set; }

    [JsonPropertyName("coin_cnt")]
    public uint CoinCount { get; set; }

    [JsonPropertyName("coin_cost")]
    public uint CoinCost { get; set; }

    [JsonPropertyName("rule")]
    public string Rule { get; set; } = default!;

    [JsonPropertyName("signed")]
    public bool Signed { get; set; }

    [JsonPropertyName("sign_days")]
    public uint SignDays { get; set; }

    [JsonPropertyName("cost")]
    public uint Cost { get; set; }

    [JsonPropertyName("month_quality_cnt")]
    public uint MonthQualityCount { get; set; }

    [JsonPropertyName("quality_cnt")]
    public uint QualityCount { get; set; }
}