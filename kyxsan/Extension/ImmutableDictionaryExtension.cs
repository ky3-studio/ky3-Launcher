//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace kyxsan.Extension;

internal static class ImmutableDictionaryExtension
{
    extension<TKey, TSource>(IEnumerable<TSource> source)
        where TKey : notnull
    {
        [Pure]
        public ImmutableDictionary<TKey, TSource> ToImmutableDictionaryIgnoringDuplicateKeys(Func<TSource, TKey> keySelector)
        {
            ImmutableDictionary<TKey, TSource>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TSource>();

            foreach (TSource value in source)
            {
                builder[keySelector(value)] = value;
            }

            return builder.ToImmutable();
        }

        [Pure]
        public ImmutableDictionary<TKey, TValue> ToImmutableDictionaryIgnoringDuplicateKeys<TValue>(Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();

            foreach (TSource value in source)
            {
                builder[keySelector(value)] = valueSelector(value);
            }

            return builder.ToImmutable();
        }
    }
}