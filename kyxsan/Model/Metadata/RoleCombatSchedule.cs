//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata;

internal sealed class RoleCombatSchedule : IDefaultIdentity<RoleCombatScheduleId>
{
    public required RoleCombatScheduleId Id { get; init; }

    public required DateTimeOffset Begin { get; init; }

    public required DateTimeOffset End { get; init; }

    public required ImmutableArray<ElementType> Elements { get; init; }

    public required ImmutableArray<AvatarId> SpecialAvatars { get; init; }

    public required ImmutableArray<AvatarId> InitialAvatars { get; init; }
}