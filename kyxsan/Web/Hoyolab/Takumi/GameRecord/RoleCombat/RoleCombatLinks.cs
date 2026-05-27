//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatLinks
{
    [JsonPropertyName("lineup_link")]
    public required string LineupLink { get; init; }

    [JsonPropertyName("lineup_link_pc")]
    public required string LineupLinkPC { get; init; }

    [JsonPropertyName("strategy_link")]
    public required string StrategyLink { get; init; }

    [JsonPropertyName("lineup_publish_link")]
    public required string LineupPublishLink { get; init; }

    [JsonPropertyName("lineup_publish_link_pc")]
    public required string LineupPublishLinkPC { get; init; }
}