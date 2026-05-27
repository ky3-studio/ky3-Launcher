//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Monster;
using kyxsan.Model.Metadata.Tower;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.SpiralAbyss;

internal sealed class SpiralAbyssMetadataContext : IMetadataContext,
    IMetadataDictionaryIdTowerScheduleSource,
    IMetadataDictionaryIdTowerFloorSource,
    IMetadataDictionaryIdArrayTowerLevelSource,
    IMetadataDictionaryIdMonsterSource,
    IMetadataDictionaryIdAvatarWithPlayersSource
{
    public ImmutableDictionary<TowerScheduleId, TowerSchedule> IdTowerScheduleMap { get; set; } = default!;

    public ImmutableDictionary<TowerFloorId, TowerFloor> IdTowerFloorMap { get; set; } = default!;

    public ImmutableDictionary<TowerLevelGroupId, ImmutableArray<TowerLevel>> IdArrayTowerLevelMap { get; set; } = default!;

    public ImmutableDictionary<MonsterDescribeId, Monster> IdMonsterMap { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}