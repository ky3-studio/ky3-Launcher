//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections;
using System.Collections.Immutable;

namespace kyxsan.Core.Collection.Generic;

internal sealed class ImmutableArrayEnumeratorNoThrow<T> : IEnumerator<T>
{
    private readonly ImmutableArray<T> array;
    private int index;

    public ImmutableArrayEnumeratorNoThrow(ImmutableArray<T> array)
    {
        this.array = array;
        index = -1;
    }

    public T Current { get => index < array.Length ? array[index] : default!; }

    object? IEnumerator.Current { get => Current; }

    public bool MoveNext()
    {
        return ++index < array.Length;
    }

    public void Reset()
    {
        index = -1;
    }

    public void Dispose()
    {
    }
}