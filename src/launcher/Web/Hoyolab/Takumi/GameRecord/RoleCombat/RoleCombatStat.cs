//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatStat
{
    [JsonPropertyName("difficulty_id")]
    public required RoleCombatDifficultyLevel DifficultyId { get; init; }

    [JsonPropertyName("max_round_id")]
    public required uint MaxRoundId { get; init; }

    [JsonPropertyName("heraldry")]
    public required RoleCombatDifficultyLevel Heraldry { get; init; }

    [JsonPropertyName("get_medal_round_list")]
    public required ImmutableArray<int> GetMedalRoundList { get; init; }

    [JsonPropertyName("medal_num")]
    public required int MedalNumber { get; init; }

    [JsonPropertyName("coin_num")]
    public required int CoinNumber { get; init; }

    [JsonPropertyName("avatar_bonus_num")]
    public required int AvatarBonusNumber { get; init; }

    [JsonPropertyName("rent_cnt")]
    public required int RentCount { get; init; }

    [JsonPropertyName("tarot_finished_cnt")]
    public int TarotFinishedCount { get; set; }
}