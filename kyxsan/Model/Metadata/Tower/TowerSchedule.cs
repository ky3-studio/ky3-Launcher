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

internal sealed class TowerSchedule : IDefaultIdentity<TowerScheduleId>
{
    public required TowerScheduleId Id { get; init; }

    public required ImmutableArray<TowerFloorId> FloorIds { get; init; }

    public required DateTimeOffset Open { get; init; }

    public required DateTimeOffset Close { get; init; }

    public required string BuffName { get; init; }

    public required ImmutableArray<string> Descriptions { get; init; }

    public required string Icon { get; init; }
}