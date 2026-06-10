//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Converter;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Monster;

internal sealed class MonsterBaseValue : BaseValue
{
    public required float FireSubHurt { get; init; }

    public required float GrassSubHurt { get; init; }

    public required float WaterSubHurt { get; init; }

    public required float ElecSubHurt { get; init; }

    public required float WindSubHurt { get; init; }

    public required float IceSubHurt { get; init; }

    public required float RockSubHurt { get; init; }

    public required float PhysicalSubHurt { get; init; }

    public ImmutableArray<NameStringValue> SubHurts
    {
        get
        {
            return !field.IsDefault ? field : field =
            [
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_FIRE_SUB_HURT, FireSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_WATER_SUB_HURT, WaterSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_GRASS_SUB_HURT, GrassSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_ELEC_SUB_HURT, ElecSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_WIND_SUB_HURT, WindSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_ICE_SUB_HURT, IceSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_ROCK_SUB_HURT, RockSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_PHYSICAL_SUB_HURT, PhysicalSubHurt),
            ];
        }
    }
}