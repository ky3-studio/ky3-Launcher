//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Tower;
using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.SpiralAbyss;

internal sealed partial class FloorView : IPropertyValuesProvider
{
    public FloorView(TowerFloor floor, SpiralAbyssMetadataContext context)
    {
        Index = SH.FormatModelBindingkyxsanComplexRankFloor(floor.Index);
        IndexValue = floor.Index;
        Disorders = floor.Descriptions;
        UpDisorders = floor.FirstDescriptions.EmptyIfDefault();
        DownDisorders = floor.SecondDescriptions.EmptyIfDefault();

        Levels = [.. context.IdArrayTowerLevelMap[floor.LevelGroupId].OrderBy(l => l.Index).Select(l => LevelView.Create(l, context))];
    }

    public bool Engaged { get; private set; }

    public string Index { get; }

    public string? SettleTime { get; private set; }

    public int Star { get; private set; }

    public ImmutableArray<string> Disorders { get; }

    public ImmutableArray<string> UpDisorders { get; }

    public ImmutableArray<string> DownDisorders { get; }

    public ImmutableArray<LevelView> Levels { get; }

    internal uint IndexValue { get; }

    public static FloorView Create(TowerFloor floor, SpiralAbyssMetadataContext context)
    {
        return new(floor, context);
    }

    public void Attach(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssFloor floor, TimeSpan offset, SpiralAbyssMetadataContext context)
    {
        SettleTime = $"{DateTimeOffset.FromUnixTimeSeconds(floor.SettleTime).ToOffset(offset):yyyy.MM.dd HH:mm:ss}";
        Star = floor.Star;
        Engaged = true;

        foreach (ref readonly LevelView levelView in Levels.AsSpan())
        {
            uint levelIndex = levelView.IndexValue;
            if (floor.Levels.SingleOrDefault(l => l.Index == levelIndex) is { } level)
            {
                levelView.Attach(level, offset, context);
            }
        }
    }
}