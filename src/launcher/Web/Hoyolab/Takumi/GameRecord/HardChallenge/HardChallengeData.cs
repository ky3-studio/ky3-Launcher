//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeData
{
    [JsonPropertyName("schedule")]
    public required HardChallengeSchedule Schedule { get; init; }

    [JsonPropertyName("single")]
    public required HardChallengeDataEntry SinglePlayer { get; init; }

    [JsonPropertyName("mp")]
    public required HardChallengeDataEntry MultiPlayer { get; init; }

    [JsonPropertyName("blings")]
    public required ImmutableArray<HardChallengeBlingAvatar> Blings { get; init; }
}