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

internal sealed class RoleCombatDetail
{
    [JsonPropertyName("rounds_data")]
    public required ImmutableArray<RoleCombatRoundData> RoundsData { get; init; }

    [JsonPropertyName("detail_stat")]
    public required RoleCombatStat? DetailStat { get; init; }

    [JsonPropertyName("lineup_link")]
    public Uri? LineupLink { get; init; }

    [JsonPropertyName("backup_avatars")]
    public required ImmutableArray<RoleCombatAvatar> BackupAvatars { get; init; }

    // sic
    [JsonPropertyName("fight_statisic")]
    public required RoleCombatFightStatistics FightStatistics { get; init; }
}