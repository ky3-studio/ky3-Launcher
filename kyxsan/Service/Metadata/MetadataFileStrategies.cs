//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Service.Metadata;

internal static class MetadataFileStrategies
{
    public static readonly MetadataFileStrategy Achievement = new("Achievement");
    public static readonly MetadataFileStrategy AchievementGoal = new("AchievementGoal");
    public static readonly MetadataFileStrategy Avatar = new("Avatar", true);
    public static readonly MetadataFileStrategy AvatarCurve = new("AvatarCurve");
    public static readonly MetadataFileStrategy AvatarPromote = new("AvatarPromote");
    public static readonly MetadataFileStrategy BeyondItem = new("BeyondItem");
    public static readonly MetadataFileStrategy Chapter = new("Chapter");
    public static readonly MetadataFileStrategy Combine = new("Combine");
    public static readonly MetadataFileStrategy DisplayItem = new("DisplayItem");
    public static readonly MetadataFileStrategy Furniture = new("Furniture");
    public static readonly MetadataFileStrategy FurnitureMake = new("FurnitureMake");
    public static readonly MetadataFileStrategy FurnitureSuite = new("FurnitureSuite");
    public static readonly MetadataFileStrategy FurnitureType = new("FurnitureType");
    public static readonly MetadataFileStrategy GachaEvent = new("GachaEvent");
    public static readonly MetadataFileStrategy HardChallengeSchedule = new("HardChallengeSchedule");
    public static readonly MetadataFileStrategy HyperLinkName = new("HyperLinkName");
    public static readonly MetadataFileStrategy Material = new("Material");
    public static readonly MetadataFileStrategy Meta = new("Meta");
    public static readonly MetadataFileStrategy Monster = new("Monster");
    public static readonly MetadataFileStrategy MonsterCurve = new("MonsterCurve");
    public static readonly MetadataFileStrategy NameCard = new("NameCard");
    public static readonly MetadataFileStrategy ProfilePicture = new("ProfilePicture");
    public static readonly MetadataFileStrategy Reliquary = new("Reliquary");
    public static readonly MetadataFileStrategy ReliquaryAffixWeight = new("ReliquaryAffixWeight");
    public static readonly MetadataFileStrategy ReliquaryMainAffix = new("ReliquaryMainAffix");
    public static readonly MetadataFileStrategy ReliquaryMainAffixLevel = new("ReliquaryMainAffixLevel");
    public static readonly MetadataFileStrategy ReliquarySet = new("ReliquarySet");
    public static readonly MetadataFileStrategy ReliquarySubAffix = new("ReliquarySubAffix");
    public static readonly MetadataFileStrategy RoleCombatSchedule = new("RoleCombatSchedule");
    public static readonly MetadataFileStrategy TowerFloor = new("TowerFloor");
    public static readonly MetadataFileStrategy TowerLevel = new("TowerLevel");
    public static readonly MetadataFileStrategy TowerSchedule = new("TowerSchedule");
    public static readonly MetadataFileStrategy Weapon = new("Weapon");
    public static readonly MetadataFileStrategy WeaponCurve = new("WeaponCurve");
    public static readonly MetadataFileStrategy WeaponPromote = new("WeaponPromote");
}