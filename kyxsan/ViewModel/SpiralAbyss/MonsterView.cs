//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Metadata.Tower;
using kyxsan.Model.Primitive;
using kyxsan.Web.Endpoint.kyxsan;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.SpiralAbyss;

internal sealed class MonsterView : INameIcon<Uri>
{
    private MonsterView(in MonsterDescribeId id)
    {
        Name = $"Unknown {id}";
        Icon = StaticResourcesEndpoints.UIIconNone;
        Affixes = [];
        Count = 1;
    }

    private MonsterView(TowerMonster towerMonster, Model.Metadata.Monster.Monster metaMonster)
    {
        Name = metaMonster.Name ?? $"Unknown {towerMonster.Id}";
        Icon = MonsterIconConverter.IconNameToUri(metaMonster.Icon);
        Affixes = towerMonster.Affixes.EmptyIfDefault();
        Count = (int)towerMonster.Count;
        AttackMonolith = towerMonster.AttackMonolith;
    }

    public string Name { get; }

    public Uri Icon { get; }

    public ImmutableArray<NameDescription> Affixes { get; }

    public int Count { get; }

    public bool IsCountOne { get => Count == 1; }

    public bool AttackMonolith { get; }

    public static MonsterView Default(in MonsterDescribeId id)
    {
        return new(id);
    }

    public static MonsterView Create(TowerMonster tower, Model.Metadata.Monster.Monster meta)
    {
        return new(tower, meta);
    }
}