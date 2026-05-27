//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class Reliquary
{
    [JsonPropertyName("id")]
    public ReliquaryId Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("pos")]
    public EquipType Position { get; set; }

    [JsonPropertyName("rarity")]
    public QualityType Rarity { get; set; }

    [JsonPropertyName("level")]
    public uint Level { get; set; }

    [JsonPropertyName("set")]
    public ReliquarySet ReliquarySet { get; set; } = default!;

    [JsonPropertyName("pos_name")]
    public string PositionName { get; set; } = default!;

    [JsonPropertyName("main_property")]
    public ReliquaryProperty MainProperty { get; set; } = default!;

    [JsonPropertyName("sub_property_list")]
    public ImmutableArray<ReliquaryProperty> SubPropertyList { get; set; }
}