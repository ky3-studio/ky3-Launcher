//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class DetailedCharacter
{
    [JsonPropertyName("base")]
    public Character Base { get; init; } = default!;

    [JsonPropertyName("weapon")]
    public DetailedWeapon Weapon { get; init; } = default!;

    [JsonPropertyName("relics")]
    public ImmutableArray<Reliquary> Relics { get; init; }

    [JsonPropertyName("constellations")]
    public ImmutableArray<Constellation> Constellations { get; init; }

    [JsonPropertyName("costumes")]
    public List<Costume>? Costumes { get; init; }

    [JsonPropertyName("selected_properties")]
    public ImmutableArray<BaseProperty> SelectedProperties { get; init; }

    [JsonPropertyName("skills")]
    public ImmutableArray<Skill> Skills { get; init; }

    [JsonPropertyName("recommend_relic_property")]
    public RecommendRelicProperty RecommendRelicProperty { get; init; } = default!;
}