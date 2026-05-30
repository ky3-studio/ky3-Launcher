//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Metadata.Tower;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.SpiralAbyss;

internal sealed class BattleView
{
    private BattleView(TowerLevel towerLevel, uint battleIndex, SpiralAbyssMetadataContext context)
    {
        IndexValue = battleIndex;
        Gadget = battleIndex switch
        {
            1U => towerLevel.FirstGadget,
            2U => towerLevel.SecondGadget,
            _ => default,
        };
        MonsterWaves = battleIndex switch
        {
            1U => towerLevel.FirstWaves.SelectAsArray(static (w, context) => BattleWave.Create(w, context), context),
            2U => towerLevel.SecondWaves.SelectAsArray(static (w, context) => BattleWave.Create(w, context), context),
            _ => default!,
        };
    }

    public string? Time { get; private set; }

    public ImmutableArray<AvatarView> Avatars { get; private set; } = [];

    public NameDescription? Gadget { get; }

    public ImmutableArray<BattleWave> MonsterWaves { get; }

    internal uint IndexValue { get; }

    public static BattleView Create(TowerLevel level, uint index, SpiralAbyssMetadataContext context)
    {
        return new(level, index, context);
    }

    public void Attach(SpiralAbyssBattle battle, TimeSpan offset, SpiralAbyssMetadataContext context)
    {
        Time = $"{DateTimeOffset.FromUnixTimeSeconds(battle.Timestamp).ToOffset(offset):yyyy.MM.dd HH:mm:ss}";
        Avatars = battle.Avatars.SelectAsArray(static (a, context) => AvatarView.From(context.GetAvatar(a.Id)), context);
    }
}