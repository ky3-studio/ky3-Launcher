//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class RecommendProperties
{
    [JsonPropertyName("sand_main_property_list")]
    public ImmutableArray<FightProperty> SandMainPropertyList { get; set; }

    [JsonPropertyName("goblet_main_property_list")]
    public ImmutableArray<FightProperty> GobletMainPropertyList { get; set; }

    [JsonPropertyName("circlet_main_property_list")]
    public ImmutableArray<FightProperty> CircletMainPropertyList { get; set; }

    [JsonPropertyName("sub_property_list")]
    public ImmutableArray<FightProperty> SubPropertyList { get; set; }
}