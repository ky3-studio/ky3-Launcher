// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Model.Metadata.Item;

namespace Launcher.ViewModel.Cultivation;

internal sealed partial class StatisticsCultivateItem : ObservableObject
{
    public Material Material { get; }

    public bool IsToday { get; }

    [ObservableProperty]
    public partial uint Count { get; set; }

    [ObservableProperty]
    public partial uint Current { get; set; }

    public bool IsFinished => Current >= Count;

    public string Name => Material.Name;

    public string Icon => Material.Icon;

    public string CountText => $"{Current}/{Count}";

    private StatisticsCultivateItem(Material material, bool isToday)
    {
        Material = material;
        IsToday = isToday;
    }

    public static StatisticsCultivateItem Create(Material material, TimeSpan offset)
    {
        return new(material, material.IsItemOfToday(offset));
    }

    public static StatisticsCultivateItem Create(Material material, uint count, TimeSpan offset)
    {
        StatisticsCultivateItem item = new(material, material.IsItemOfToday(offset))
        {
            Count = count,
        };
        return item;
    }
}
