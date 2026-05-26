//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
using kyxsan.Model.Primitive;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeBestAvatar
{
    [JsonPropertyName("avatar_id")]
    public required AvatarId AvatarId { get; init; }

    [JsonPropertyName("side_icon")]
    public required Uri SideIcon { get; init; }

    /// <summary>
    /// 实际上是总值或最值
    /// </summary>
    [JsonPropertyName("dps")]
    public required int Dps { get; init; }

    [JsonPropertyName("type")]
    public required HardChallengeBestAvatarType Type { get; init; }
}