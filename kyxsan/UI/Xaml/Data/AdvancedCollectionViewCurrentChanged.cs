//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace kyxsan.UI.Xaml.Data;

internal static class AdvancedCollectionViewCurrentChanged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Attach<T>(IAdvancedCollectionView<T>? acv, EventHandler<object> handler)
        where T : class
    {
        if (acv is null)
        {
            return;
        }

        acv.CurrentChanged += handler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Detach<T>(IAdvancedCollectionView<T>? acv, EventHandler<object> handler)
        where T : class
    {
        if (acv is null)
        {
            return;
        }

        acv.CurrentChanged -= handler;
    }
}