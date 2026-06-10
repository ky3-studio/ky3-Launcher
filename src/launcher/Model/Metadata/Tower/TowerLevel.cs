//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Tower;

internal sealed class TowerLevel
{
    public required TowerLevelId Id { get; init; }

    public required TowerLevelGroupId GroupId { get; init; }

    public required uint Index { get; init; }

    public required uint MonsterLevel { get; init; }

    public ImmutableArray<MonsterDescribeId> FirstMonsters { get; init; } = [];

    public ImmutableArray<TowerWave> FirstWaves { get; init; } = [];

    public NameDescription? FirstGadget { get; init; }

    public ImmutableArray<MonsterDescribeId> SecondMonsters { get; init; } = [];

    public ImmutableArray<TowerWave> SecondWaves { get; init; } = [];

    public NameDescription? SecondGadget { get; init; }
}