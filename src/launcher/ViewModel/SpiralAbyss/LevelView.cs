//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Tower;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.SpiralAbyss;

internal sealed class LevelView
{
    private LevelView(TowerLevel towerLevel, SpiralAbyssMetadataContext context)
    {
        Index = SH.FormatModelBindingkyxsanComplexRankLevel(towerLevel.Index);
        IndexValue = towerLevel.Index;
        Battles = [BattleView.Create(towerLevel, 1, context), BattleView.Create(towerLevel, 2, context)];
    }

    public string Index { get; }

    public int Star { get; private set; }

    public ImmutableArray<BattleView> Battles { get; }

    internal uint IndexValue { get; }

    public static LevelView Create(TowerLevel towerLevel, SpiralAbyssMetadataContext context)
    {
        return new(towerLevel, context);
    }

    public void Attach(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssLevel level, TimeSpan offset, SpiralAbyssMetadataContext context)
    {
        Star = level.Star;

        foreach (ref readonly BattleView battleView in Battles.AsSpan())
        {
            uint battleIndex = battleView.IndexValue;
            if (level.Battles.SingleOrDefault(b => b.Index == battleIndex) is { } battle)
            {
                battleView.Attach(battle, offset, context);
            }
        }
    }
}