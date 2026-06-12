// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model.Entity;
using kyxsan.Model.Metadata.Item;

namespace kyxsan.ViewModel.Cultivation;

internal sealed partial class InventoryItemView : ObservableObject
{
    public InventoryItem Entity { get; }

    public Material Material { get; }

    [ObservableProperty]
    public partial uint Count { get; set; }

    public string Name => Material.Name;

    public string Icon => Material.Icon;

    private InventoryItemView(InventoryItem entity, Material material)
    {
        Entity = entity;
        Material = material;
        Count = entity.Count;
    }

    partial void OnCountChanged(uint value)
    {
        Entity.Count = value;
    }

    public static InventoryItemView Create(InventoryItem item, Material material)
    {
        return new(item, material);
    }
}
