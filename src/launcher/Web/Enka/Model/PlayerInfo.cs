//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.Web.Enka.Model;

internal sealed class PlayerInfo
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("level")]
    public uint Level { get; set; }

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = default!;

    [JsonPropertyName("worldLevel")]
    public uint WorldLevel { get; set; }

    [JsonPropertyName("nameCardId")]
    public MaterialId NameCardId { get; set; }

    [JsonPropertyName("finishAchievementNum")]
    public uint FinishAchievementNum { get; set; }

    [JsonPropertyName("towerFloorIndex")]
    public uint TowerFloorIndex { get; set; }

    [JsonPropertyName("towerLevelIndex")]
    public uint TowerLevelIndex { get; set; }

    [JsonPropertyName("showAvatarInfoList")]
    public ImmutableArray<ShowAvatarInfo> ShowAvatarInfoList { get; set; }

    [JsonPropertyName("showNameCardIdList")]
    public ImmutableArray<MaterialId> ShowNameCardIdList { get; set; }

    [JsonPropertyName("profilePicture")]
    public ProfilePicture ProfilePicture { get; set; } = default!;

    [JsonPropertyName("theaterActIndex")]
    public uint TheaterActIndex { get; set; }

    [JsonPropertyName("theaterModeIndex")]
    public uint TheaterModeIndex { get; set; }

    [JsonPropertyName("theaterStarIndex")]
    public uint TheaterStarIndex { get; set; }

    [JsonPropertyName("fetterCount")]
    public uint FetterCount { get; set; }

    [JsonPropertyName("towerStarIndex")]
    public uint TowerStarIndex { get; set; }
}