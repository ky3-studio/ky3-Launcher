//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace kyxsan.Extension;

internal static class EnumerableExtension
{
    public static IEnumerable<KeyValuePair<TKey, int>> CountByKey<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> source, Func<TValue, bool> predicate)
        where TKey : notnull
    {
        return source.SelectMany(map => map.Where(kv => predicate(kv.Value))).CountBy(kv => kv.Key);
    }

    extension<T>(IEnumerable<T> source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObservableCollection<T> ToObservableCollection()
        {
            return new(source);
        }

        public string ToString(char separator)
        {
            return string.Join(separator, source);
        }
    }
}