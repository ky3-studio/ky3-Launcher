//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Launcher.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeChallenge
{
    /// <summary>
    /// ¹ÖÎïÃû³Æ
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("second")]
    public required int Seconds { get; init; }

    [JsonPropertyName("teams")]
    public required ImmutableArray<HardChallengeAvatar> Team { get; init; }

    [JsonPropertyName("best_avatar")]
    public required ImmutableArray<HardChallengeBestAvatar> BestAvatars { get; init; }

    [JsonPropertyName("monster")]
    public required HardChallengeMonster Monster { get; init; }
}