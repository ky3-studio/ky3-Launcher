// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.Service.Cultivation;

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
