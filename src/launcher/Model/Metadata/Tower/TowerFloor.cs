//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Primitive;
using System.Collections.Immutable;

namespace Launcher.Model.Metadata.Tower;

internal sealed class TowerFloor : IDefaultIdentity<TowerFloorId>
{
    public required TowerFloorId Id { get; init; }

    public required uint Index { get; init; }

    public required TowerLevelGroupId LevelGroupId { get; init; }

    public required string Background { get; init; }

    public required ImmutableArray<string> Descriptions { get; init; }

    public ImmutableArray<string> FirstDescriptions { get; init; } = [];

    public ImmutableArray<string> SecondDescriptions { get; init; } = [];
}