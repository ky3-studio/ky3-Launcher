//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model.Intrinsic.Format;

internal static class FormatMethodExtension
{
    extension(FightProperty value)
    {
        internal FormatMethod GetFormatMethod()
        {
            return value switch
            {
                FightProperty.FIGHT_PROP_BASE_HP => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_HP => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_HP_PERCENT => FormatMethod.Percent,

                FightProperty.FIGHT_PROP_BASE_ATTACK => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_ATTACK => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_ATTACK_PERCENT => FormatMethod.Percent,

                FightProperty.FIGHT_PROP_BASE_DEFENSE => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_DEFENSE => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_DEFENSE_PERCENT => FormatMethod.Percent,

                FightProperty.FIGHT_PROP_CRITICAL => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_CRITICAL_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_HEAL_ADD => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ELEMENT_MASTERY => FormatMethod.Integer,

                FightProperty.FIGHT_PROP_PHYSICAL_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT => FormatMethod.Percent,

                FightProperty.FIGHT_PROP_FIRE_ADD_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ELEC_ADD_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_WATER_ADD_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_GRASS_ADD_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_WIND_ADD_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ROCK_ADD_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ICE_ADD_HURT => FormatMethod.Percent,

                FightProperty.FIGHT_PROP_FIRE_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ELEC_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_WATER_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_GRASS_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_WIND_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ROCK_SUB_HURT => FormatMethod.Percent,
                FightProperty.FIGHT_PROP_ICE_SUB_HURT => FormatMethod.Percent,

                FightProperty.FIGHT_PROP_MAX_HP => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_CUR_ATTACK => FormatMethod.Integer,
                FightProperty.FIGHT_PROP_CUR_DEFENSE => FormatMethod.Integer,
                _ => FormatMethod.None,
            };
        }
    }
}