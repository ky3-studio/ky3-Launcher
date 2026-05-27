//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace kyxsan.Service.kyxsan;

internal abstract class StatisticsCache
{
    private readonly ConcurrentDictionary<Type, TaskCompletionSource> typeToTcs = [];

    protected static IEnumerable<TResult> CurrentJoinLast<TElement, TKey, TResult>(IEnumerable<TElement> current, IEnumerable<TElement>? last, Func<TElement, TKey> keySelector, Func<TElement, TElement?, TResult> resultSelector)
        where TKey : notnull
    {
        if (last is null)
        {
            foreach (TElement element in current)
            {
                yield return resultSelector(element, default);
            }
        }
        else
        {
            Dictionary<TKey, TElement> lastMap = [];
            foreach (TElement element in last)
            {
                lastMap[keySelector(element)] = element;
            }

            foreach (TElement element in current)
            {
                yield return resultSelector(element, lastMap.GetValueOrDefault(keySelector(element)));
            }
        }
    }

    protected async ValueTask InitializeForTypeAsync<T, TContext>(TContext context, Func<TContext, Task> initialization)
    {
        if (typeToTcs.TryGetValue(typeof(T), out TaskCompletionSource? tcs))
        {
            await tcs.Task.ConfigureAwait(false);
            return;
        }

        tcs = new();
        if (typeToTcs.TryAdd(typeof(T), tcs))
        {
            await initialization(context).ConfigureAwait(false);
            tcs.TrySetResult();
        }
    }
}