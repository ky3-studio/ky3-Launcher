//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Web.Endpoint.Launcher;
using System.Collections.Frozen;

namespace Launcher.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class FightPropertyToIconConverter : ValueConverter<FightProperty, Uri?>
{
    private static readonly FrozenDictionary<FightProperty, Uri> PropertyIcons = FrozenDictionary.ToFrozenDictionary(
    (KeyValuePair<FightProperty, Uri>[])
    [
        KeyValuePair.Create(FightProperty.FIGHT_PROP_SKILL_CD_MINUS_RATIO, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_CDReduce.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_ChargeEfficiency.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CRITICAL, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Critical.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CRITICAL_HURT, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Critical_Hurt.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CUR_ATTACK, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_CurAttack.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CUR_DEFENSE, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_CurDefense.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEMENT_MASTERY, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Element.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEC_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Electric.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_FIRE_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Fire.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_GRASS_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Grass.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ICE_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Ice.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ROCK_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Rock.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WATER_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Water.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WIND_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Wind.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_HEAL_ADD, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Heal.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_MAX_HP, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_MaxHp.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_PhysicalAttackUp.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_SHIELD_COST_MINUS_RATIO, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_ShieldCostMinus.png").ToUri()),
    ]);

    public override Uri? Convert(FightProperty from)
    {
        return PropertyIcons.GetValueOrDefault(from);
    }
}
