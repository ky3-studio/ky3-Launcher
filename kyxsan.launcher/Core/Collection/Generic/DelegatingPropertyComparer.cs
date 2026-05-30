//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace kyxsan.Core.Collection.Generic;

internal abstract class DelegatingPropertyComparer<T, TProperty> : IComparer<T>
    where T : class
{
    private readonly IComparer<TProperty> delegatedComparer;
    private readonly Func<T, TProperty> delegation;

    protected DelegatingPropertyComparer([RequireStaticDelegate] Func<T, TProperty> delegation, IComparer<TProperty> delegatedComparer)
    {
        this.delegation = delegation;
        this.delegatedComparer = delegatedComparer;
    }

    public int Compare(T? x, T? y)
    {
        return (x, y) switch
        {
            (null, not null) => -1,
            (not null, null) => 1,
            (null, null) => 0,
            (not null, not null) => delegatedComparer.Compare(delegation(x), delegation(y)),
        };
    }
}