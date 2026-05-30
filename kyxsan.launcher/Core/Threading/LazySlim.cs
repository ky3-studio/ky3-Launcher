//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Threading;

internal static class LazySlim
{
    public static LazySlim<T> Create<T>()
        where T : new()
    {
        return new(static () => new T());
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class LazySlim<T>
{
    private readonly Func<T> valueFactory;

    private bool isValueCreated;
    private object? syncRoot;

    public LazySlim(Func<T> valueFactory)
    {
        this.valueFactory = valueFactory;
    }

    [field: MaybeNull]
    public T Value { get => LazyInitializer.EnsureInitialized(ref field, ref isValueCreated, ref syncRoot, valueFactory); }

    public bool IsValueCreated { get => isValueCreated; }
}