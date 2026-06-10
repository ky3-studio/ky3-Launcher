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

internal static class AdvancedCollectionViewExtension
{
    extension<T>(IEnumerable<T> source)
        where T : class, IPropertyValuesProvider
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAdvancedCollectionView<T> AsAdvancedCollectionView()
        {
            return source switch
            {
                IAdvancedCollectionView<T> advancedCollectionView => advancedCollectionView,
                IList<T> list => new AdvancedCollectionView<T>(list),
                _ => new AdvancedCollectionView<T>([.. source]),
            };
        }
    }
}