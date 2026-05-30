//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatData
{
    [JsonPropertyName("detail")]
    public required RoleCombatDetail Detail { get; init; }

    [JsonPropertyName("stat")]
    public required RoleCombatStat Stat { get; init; }

    [JsonPropertyName("schedule")]
    public required RoleCombatSchedule Schedule { get; init; }

    [JsonPropertyName("has_data")]
    public required bool HasData { get; init; }

    [JsonPropertyName("has_detail_data")]
    public required bool HasDetailData { get; init; }
}