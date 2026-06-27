//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Monster;
using Launcher.Model.Metadata.Tower;
using Launcher.Model.Primitive;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Launcher.ViewModel.SpiralAbyss;

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