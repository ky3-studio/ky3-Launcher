//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database.Abstraction;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.Database;

internal static class SelectableExtension
{
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : ISelectable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TSource? SelectedOrFirstOrDefault()
        {
            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return default;
                }

                TSource first = e.Current;

                do
                {
                    TSource result = e.Current;
                    if (!result.IsSelected)
                    {
                        continue;
                    }

                    while (e.MoveNext())
                    {
                        if (e.Current.IsSelected)
                        {
                            return default;
                        }
                    }

                    return result;
                }
                while (e.MoveNext());

                return first;
            }
        }
    }
}