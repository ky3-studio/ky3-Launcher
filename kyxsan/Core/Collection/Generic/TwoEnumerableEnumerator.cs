//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace kyxsan.Core.Collection.Generic;

internal sealed partial class TwoEnumerableEnumerator<TFirst, TSecond> : IDisposable
{
    private readonly IEnumerator<TFirst> firstEnumerator;
    private readonly IEnumerator<TSecond> secondEnumerator;

    public TwoEnumerableEnumerator(IEnumerable<TFirst> firstEnumerable, IEnumerable<TSecond> secondEnumerable)
    {
        firstEnumerator = GetNoThrowEnumeratorIfPossible(firstEnumerable);
        secondEnumerator = GetNoThrowEnumeratorIfPossible(secondEnumerable);
    }

    public (TFirst? First, TSecond? Second) Current { get => (firstEnumerator.Current, secondEnumerator.Current); }

    public bool MoveNext(ref bool moveFirst, ref bool moveSecond)
    {
        moveFirst = moveFirst && firstEnumerator.MoveNext();
        moveSecond = moveSecond && secondEnumerator.MoveNext();

        return moveFirst || moveSecond;
    }

    public void Dispose()
    {
        firstEnumerator.Dispose();
        secondEnumerator.Dispose();
    }

    [MustDisposeResource]
    private static IEnumerator<T> GetNoThrowEnumeratorIfPossible<T>(IEnumerable<T> enumerable)
    {
        if (enumerable is ImmutableArray<T> immutableArray)
        {
            return new ImmutableArrayEnumeratorNoThrow<T>(immutableArray);
        }

        return enumerable.GetEnumerator();
    }
}