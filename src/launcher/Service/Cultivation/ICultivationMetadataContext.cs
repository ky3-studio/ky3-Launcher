// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Model;
using Launcher.Model.Metadata;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Item;
using Launcher.Model.Metadata.Weapon;
using Launcher.Model.Primitive;
using System.Collections.Immutable;

namespace Launcher.Service.Cultivation;

internal interface ICultivationMetadataContext
{
    ImmutableArray<Avatar> Avatars { get; }

    ImmutableArray<Material> Materials { get; }

    ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; }

    ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; }

    ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; }

    ImmutableDictionary<MaterialId, Combine> ResultMaterialIdCombineMap { get; }

    Material GetMaterial(MaterialId materialId);

    Item GetAvatarItem(AvatarId avatarId);

    Item GetWeaponItem(WeaponId weaponId);

    IEnumerable<Material> EnumerateInventoryMaterial();
}
