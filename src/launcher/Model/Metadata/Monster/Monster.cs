//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Primitive;
using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Monster;

internal sealed partial class Monster : IPropertyValuesProvider
{
    internal const uint MaxLevel = 110U;

    public required MonsterId Id { get; init; }

    public required MonsterDescribeId DescribeId { get; init; }

    public string? MonsterName { get; init; }

    public string? Name { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public required string Icon { get; init; }

    public required MonsterType Type { get; init; }

    public required Arkhe Arkhe { get; init; }

    public List<string>? Affixes { get; init; }

    public ImmutableArray<MaterialId> Drops { get; init; } = [];

    public MonsterBaseValue? BaseValue { get; init; }

    public TypeValueCollection<FightProperty, GrowCurveType>? GrowCurves { get; init; }

    [JsonIgnore]
    public ImmutableArray<DisplayItem>? DropsView { get; set; }
}