//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Reliquary;
using Launcher.Model.Metadata.Weapon;
using Launcher.Model.Primitive;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Launcher.Service.Launcher;

internal sealed class LauncherSpiralAbyssStatisticsMetadataContext : IMetadataContext,
    IMetadataDictionaryIdAvatarWithPlayersSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryExtendedEquipAffixIdReliquarySetSource
{
    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<ExtendedEquipAffixId, ReliquarySet> ExtendedIdReliquarySetMap { get; set; } = default!;
}
