//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.RoleCombat;

internal sealed class RoleCombatMetadataContext : IMetadataContext,
    IMetadataDictionaryIdRoleCombatScheduleSource,
    IMetadataDictionaryIdAvatarWithPlayersSource
{
    public ImmutableDictionary<RoleCombatScheduleId, RoleCombatSchedule> IdRoleCombatScheduleMap { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}