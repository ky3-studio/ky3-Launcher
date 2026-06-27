// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Model.Entity;
using Launcher.Model.Metadata.Item;

namespace Launcher.ViewModel.Cultivation;

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
