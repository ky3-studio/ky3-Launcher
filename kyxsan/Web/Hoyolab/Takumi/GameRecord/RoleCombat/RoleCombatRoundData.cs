//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatRoundData
{
    [JsonPropertyName("avatars")]
    public required ImmutableArray<RoleCombatAvatar> Avatars { get; init; }

    [JsonPropertyName("choice_cards")]
    public required ImmutableArray<RoleCombatBuff> ChoiceCards { get; init; }

    [JsonPropertyName("buffs")]
    public required ImmutableArray<RoleCombatBuff> Buffs { get; init; }

    [JsonPropertyName("is_get_medal")]
    public required bool IsGetMedal { get; init; }

    [JsonPropertyName("round_id")]
    public required uint RoundId { get; init; }

    [JsonPropertyName("finish_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long FinishTime { get; init; }

    [JsonPropertyName("finish_date_time")]
    public required DateTime FinishDateTime { get; init; }

    [JsonPropertyName("enemies")]
    public required ImmutableArray<RoleCombatEnemy> Enemies { get; init; }

    [JsonPropertyName("splendour_buff")]
    public required RoleCombatSplendourBuffWrapper SplendourBuff { get; init; }

    [JsonPropertyName("is_tarot")]
    public bool IsTarot { get; init; }

    [JsonPropertyName("tarot_serial_no")]
    public int TarotSerialNumber { get; init; }
}