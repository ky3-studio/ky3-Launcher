//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Achievement;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Metadata.Monster;
using kyxsan.Model.Metadata.Reliquary;
using kyxsan.Model.Metadata.Tower;
using kyxsan.Model.Metadata.Weapon;
using System.Collections.Immutable;

namespace kyxsan.Service.Metadata;

internal static class MetadataServiceImmutableArrayExtension
{
    extension(IMetadataService metadataService)
    {
        public ValueTask<ImmutableArray<Model.Metadata.Achievement.Achievement>> GetAchievementArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Model.Metadata.Achievement.Achievement>(MetadataFileStrategies.Achievement, token);
        }

        public ValueTask<ImmutableArray<Chapter>> GetChapterArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Chapter>(MetadataFileStrategies.Chapter, token);
        }

        public ValueTask<ImmutableArray<AchievementGoal>> GetAchievementGoalArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<AchievementGoal>(MetadataFileStrategies.AchievementGoal, token);
        }

        public ValueTask<ImmutableArray<Avatar>> GetAvatarArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Avatar>(MetadataFileStrategies.Avatar, token);
        }

        public ValueTask<ImmutableArray<GrowCurve>> GetAvatarCurveArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<GrowCurve>(MetadataFileStrategies.AvatarCurve, token);
        }

        public ValueTask<ImmutableArray<Promote>> GetAvatarPromoteArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Promote>(MetadataFileStrategies.AvatarPromote, token);
        }

        public ValueTask<ImmutableArray<DisplayItem>> GetDisplayItemArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<DisplayItem>(MetadataFileStrategies.DisplayItem, token);
        }

        public ValueTask<ImmutableArray<GachaEvent>> GetGachaEventArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<GachaEvent>(MetadataFileStrategies.GachaEvent, token);
        }

        public ValueTask<ImmutableArray<HyperLinkName>> GetHyperLinkNameArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<HyperLinkName>(MetadataFileStrategies.HyperLinkName, token);
        }

        public ValueTask<ImmutableArray<Material>> GetMaterialArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Material>(MetadataFileStrategies.Material, token);
        }

        public ValueTask<ImmutableArray<Monster>> GetMonsterArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Monster>(MetadataFileStrategies.Monster, token);
        }

        public ValueTask<ImmutableArray<GrowCurve>> GetMonsterCurveArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<GrowCurve>(MetadataFileStrategies.MonsterCurve, token);
        }

        public ValueTask<ImmutableArray<ProfilePicture>> GetProfilePictureArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<ProfilePicture>(MetadataFileStrategies.ProfilePicture, token);
        }

        public ValueTask<ImmutableArray<Reliquary>> GetReliquaryArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Reliquary>(MetadataFileStrategies.Reliquary, token);
        }

        public ValueTask<ImmutableArray<ReliquaryMainAffix>> GetReliquaryMainAffixArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<ReliquaryMainAffix>(MetadataFileStrategies.ReliquaryMainAffix, token);
        }

        public ValueTask<ImmutableArray<ReliquaryMainAffixLevel>> GetReliquaryMainAffixLevelArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<ReliquaryMainAffixLevel>(MetadataFileStrategies.ReliquaryMainAffixLevel, token);
        }

        public ValueTask<ImmutableArray<ReliquarySet>> GetReliquarySetArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<ReliquarySet>(MetadataFileStrategies.ReliquarySet, token);
        }

        public ValueTask<ImmutableArray<ReliquarySubAffix>> GetReliquarySubAffixArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<ReliquarySubAffix>(MetadataFileStrategies.ReliquarySubAffix, token);
        }

        public ValueTask<ImmutableArray<TowerFloor>> GetTowerFloorArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<TowerFloor>(MetadataFileStrategies.TowerFloor, token);
        }

        public ValueTask<ImmutableArray<TowerLevel>> GetTowerLevelArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<TowerLevel>(MetadataFileStrategies.TowerLevel, token);
        }

        public ValueTask<ImmutableArray<TowerSchedule>> GetTowerScheduleArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<TowerSchedule>(MetadataFileStrategies.TowerSchedule, token);
        }

        public ValueTask<ImmutableArray<Weapon>> GetWeaponArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Weapon>(MetadataFileStrategies.Weapon, token);
        }

        public ValueTask<ImmutableArray<GrowCurve>> GetWeaponCurveArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<GrowCurve>(MetadataFileStrategies.WeaponCurve, token);
        }

        public ValueTask<ImmutableArray<Promote>> GetWeaponPromoteArrayAsync(CancellationToken token = default)
        {
            return metadataService.FromCacheOrFileAsync<Promote>(MetadataFileStrategies.WeaponPromote, token);
        }
    }
}