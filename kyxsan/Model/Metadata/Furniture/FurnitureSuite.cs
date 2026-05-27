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

namespace kyxsan.Model.Metadata.Furniture;

internal sealed class FurnitureSuite
{
    public required FurnitureSuiteId Id { get; init; }

    public required ImmutableArray<FurnitureTypeId> Types { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string ItemIcon { get; init; }

    public string? MapIcon { get; init; }

    public required ImmutableArray<AvatarId> FavoriteNpcs { get; init; }

    public required ImmutableArray<FurnitureId> Units { get; init; }
}