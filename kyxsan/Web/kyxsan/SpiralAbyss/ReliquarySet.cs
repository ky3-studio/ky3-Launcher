//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using System.Globalization;

namespace kyxsan.Web.kyxsan.SpiralAbyss;

internal sealed class ReliquarySet : IEquatable<ReliquarySet>
{
    public ReliquarySet(string set)
        : this(set.AsSpan())
    {
    }

    public ReliquarySet(ReadOnlySpan<char> set)
    {
        if (set.TrySplitIntoTwo('-', out ReadOnlySpan<char> equipAffixId, out ReadOnlySpan<char> count))
        {
            EquipAffixId = uint.Parse(equipAffixId, CultureInfo.InvariantCulture);
            Count = int.Parse(count, CultureInfo.InvariantCulture);
        }
    }

    public ExtendedEquipAffixId EquipAffixId { get; }

    public int Count { get; }

    public bool Equals(ReliquarySet? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EquipAffixId == other.EquipAffixId && Count == other.Count;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReliquarySet);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EquipAffixId, Count);
    }

    public override string ToString()
    {
        return $"{EquipAffixId.Value}-{Count}";
    }
}
