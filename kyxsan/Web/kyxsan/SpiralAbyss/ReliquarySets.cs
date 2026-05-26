//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.kyxsan.SpiralAbyss.Converter;

namespace kyxsan.Web.kyxsan.SpiralAbyss;

[JsonConverter(typeof(ReliquarySetsConverter))]
internal sealed partial class ReliquarySets : List<ReliquarySet>, IEquatable<ReliquarySets>
{
    public ReliquarySets(IEnumerable<ReliquarySet> sets)
        : base(sets)
    {
    }

    public bool Equals(ReliquarySets? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || this.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReliquarySets);
    }

    public override int GetHashCode()
    {
        HashCode hashCode = default;
        foreach (ReliquarySet set in this)
        {
            hashCode.Add(set);
        }

        return hashCode.ToHashCode();
    }
}
