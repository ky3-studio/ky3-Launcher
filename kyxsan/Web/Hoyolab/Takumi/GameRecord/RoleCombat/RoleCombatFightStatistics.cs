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

internal sealed class RoleCombatFightStatistics
{
    [JsonPropertyName("max_defeat_avatar")]
    public RoleCombatAvatarStatistics? MaxDefeatAvatar { get; init; }

    [JsonPropertyName("max_damage_avatar")]
    public RoleCombatAvatarStatistics? MaxDamageAvatar { get; init; }

    [JsonPropertyName("max_take_damage_avatar")]
    public RoleCombatAvatarStatistics? MaxTakeDamageAvatar { get; init; }

    [JsonPropertyName("total_coin_consumed")]
    public RoleCombatAvatarStatistics? TotalCoinConsumed { get; init; }

    [JsonPropertyName("shortest_avatar_list")]
    public required ImmutableArray<RoleCombatAvatarStatistics> ShortestAvatarList { get; init; }

    [JsonPropertyName("total_use_time")]
    public required int TotalUseTime { get; init; }

    [JsonPropertyName("is_show_battle_stats")]
    public required bool IsShowBattleStats { get; init; }
}