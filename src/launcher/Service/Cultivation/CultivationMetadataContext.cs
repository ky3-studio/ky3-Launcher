// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace kyxsan.Service.Cultivation;

internal sealed class CultivationMetadataContext : ICultivationMetadataContext,
    IMetadataContext,
    IMetadataArrayAvatarSource,
    IMetadataArrayMaterialSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryIdMaterialSource,
    IMetadataDictionaryResultMaterialIdCombineSource
{
    public ImmutableArray<Avatar> Avatars { get; set; }

    public ImmutableArray<Material> Materials { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Combine> ResultMaterialIdCombineMap { get; set; } = default!;

    public Material GetMaterial(MaterialId materialId)
    {
        return IdMaterialMap.GetValueOrDefault(materialId) ?? Material.Default;
    }

    public Item GetAvatarItem(AvatarId avatarId)
    {
        if (IdAvatarMap.TryGetValue(avatarId, out Avatar? avatar))
        {
            return avatar.GetOrCreateItem();
        }

        return new() { Name = $"??? ({avatarId})", Icon = default!, Badge = default!, Quality = default };
    }

    public Item GetWeaponItem(WeaponId weaponId)
    {
        if (IdWeaponMap.TryGetValue(weaponId, out Weapon? weapon))
        {
            return weapon.GetOrCreateItem();
        }

        return new() { Name = $"??? ({weaponId})", Icon = default!, Badge = default!, Quality = default };
    }

    public IEnumerable<Material> EnumerateInventoryMaterial()
    {
        foreach (Material material in Materials)
        {
            if (material.IsInventoryItem())
            {
                yield return material;
            }
        }
    }
}
