//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Monster;
using kyxsan.Model.Metadata.Tower;
using kyxsan.Model.Primitive;
using System.Collections.Immutable;
using System.Globalization;

namespace kyxsan.ViewModel.SpiralAbyss;

internal sealed class BattleWave
{
    private BattleWave(TowerWave towerWave, SpiralAbyssMetadataContext context)
    {
        Description = towerWave.Type.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture) ?? SH.ModelMetadataTowerWaveTypeDefault;
        Monsters = towerWave.Monsters.SelectAsArray(static (m, context) => CreateMonsterViewOrDefault(m, context), context);
    }

    public string Description { get; }

    public ImmutableArray<MonsterView> Monsters { get; }

    public static BattleWave Create(TowerWave tower, SpiralAbyssMetadataContext context)
    {
        return new(tower, context);
    }

    private static MonsterView CreateMonsterViewOrDefault(TowerMonster towerMonster, SpiralAbyssMetadataContext context)
    {
        MonsterDescribeId normalizedId = MonsterDescribe.Normalize(towerMonster.Id);
        return context.IdMonsterMap.TryGetValue(normalizedId, out Monster? metadataMonster)
            ? MonsterView.Create(towerMonster, metadataMonster)
            : MonsterView.Default(normalizedId);
    }
}