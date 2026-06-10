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

namespace kyxsan.Model.Metadata.Reliquary;

internal sealed class ReliquarySet
{
    public required ReliquarySetId SetId { get; init; }

    public required EquipAffixId EquipAffixId { get; init; }

    public required ImmutableHashSet<ExtendedEquipAffixId> EquipAffixIds { get; init; }

    public required string Name { get; init; }

    public required string Icon { get; init; }

    public required ImmutableArray<int> NeedNumber { get; init; }

    public required ImmutableArray<string> Descriptions { get; init; }
}