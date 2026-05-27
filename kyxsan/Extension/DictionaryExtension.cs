//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.Extension;

internal static class DictionaryExtension
{
    public static void DecreaseByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        where TKey : notnull
        where TValue : struct, ISubtractionOperators<TValue, TValue, TValue>
    {
        ref TValue current = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
        current -= value;
    }

    public static void IncreaseByOne<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull
        where TValue : struct, IIncrementOperators<TValue>
    {
        ++CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
    }

    public static void IncreaseByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        where TKey : notnull
        where TValue : struct, IAdditionOperators<TValue, TValue, TValue>
    {
        ref TValue current = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
        current += value;
    }

    public static bool TryIncreaseByOne<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull
        where TValue : struct, IIncrementOperators<TValue>
    {
        ref TValue value = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (!Unsafe.IsNullRef(ref value))
        {
            ++value;
            return true;
        }

        return false;
    }

    [Pure]
    public static Dictionary<TKey, TSource> ToDictionaryIgnoringDuplicateKeys<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        where TKey : notnull
    {
        Dictionary<TKey, TSource> dictionary = [];

        foreach (TSource value in source)
        {
            dictionary[keySelector(value)] = value;
        }

        return dictionary;
    }

    [Pure]
    public static Dictionary<TKey, TValue> ToDictionaryIgnoringDuplicateKeys<TKey, TValue, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        Dictionary<TKey, TValue> dictionary = [];

        foreach (TSource value in source)
        {
            dictionary[keySelector(value)] = valueSelector(value);
        }

        return dictionary;
    }

    public static IDictionary<TKey, TValue> WithKeysRemoved<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IReadOnlySet<TKey> keys)
        where TKey : notnull
    {
        Dictionary<TKey, TValue> results = [];
        foreach ((TKey key, TValue value) in dictionary)
        {
            if (!keys.Contains(key))
            {
                results[key] = value;
            }
        }

        return results;
    }
}