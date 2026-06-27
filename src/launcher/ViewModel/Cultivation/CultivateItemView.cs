// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Model.Entity;
using Launcher.Model.Metadata.Item;

namespace Launcher.ViewModel.Cultivation;

internal sealed partial class CultivateItemView : ObservableObject
{
    public CultivateItem Entity { get; }

    public Material Material { get; }

    public bool IsToday { get; }

    [ObservableProperty]
    public partial bool IsFinished { get; set; }

    private CultivateItemView(CultivateItem entity, Material material, bool isToday)
    {
        Entity = entity;
        Material = material;
        IsToday = isToday;
        IsFinished = entity.IsFinished;
    }

    partial void OnIsFinishedChanged(bool value)
    {
        Entity.IsFinished = value;
    }

    public string Name => Material.Name;

    public string Icon => Material.Icon;

    public uint Count => Entity.Count;

    public static CultivateItemView Create(CultivateItem item, Material material, TimeSpan offset)
    {
        bool isToday = material.IsItemOfToday(offset);
        return new(item, material, isToday);
    }
}
