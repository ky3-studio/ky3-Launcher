//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using System.Collections.Immutable;

namespace kyxsan.Service;

internal static class Selection
{
    public static TNameValue? Initialize<TNameValue, T>(Lazy<ImmutableArray<TNameValue>> options, T current)
        where TNameValue : NameValue<T>
        where T : struct, IEquatable<T>
    {
        return options.Value.SingleOrDefault(option => option.Value.Equals(current));
    }

    public static TNameValue? Initialize<TNameValue, T>(ImmutableArray<TNameValue> options, T current)
        where TNameValue : NameValue<T>
        where T : notnull
    {
        return options.SingleOrDefault(option => option.Value.Equals(current));
    }

    public static TAny? Initialize<TAny, T>(ImmutableArray<TAny> options, T current, Func<TAny, T> valueSelector, IEqualityComparer<T> comparer)
        where T : notnull
    {
        return options.SingleOrDefault(option => comparer.Equals(valueSelector(option), current));
    }
}