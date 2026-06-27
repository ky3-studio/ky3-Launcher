//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Database.Abstraction;
using Launcher.Model;
using Launcher.UI.Xaml.Data;
using System.Runtime.CompilerServices;

namespace Launcher.Core.Database;

internal static class AdvancedDbCollectionViewExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAdvancedDbCollectionView<TEntity> ToAdvancedDbCollectionView<TEntity>(this IList<TEntity> source, IServiceProvider serviceProvider)
        where TEntity : class, IPropertyValuesProvider, ISelectable
    {
        return new AdvancedDbCollectionView<TEntity>(source, serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAdvancedDbCollectionView<TEntityAccess> ToAdvancedDbCollectionView<TEntityAccess, TEntity>(this IList<TEntityAccess> source, IServiceProvider serviceProvider)
        where TEntityAccess : class, IEntityAccess<TEntity>, IPropertyValuesProvider
        where TEntity : class, ISelectable
    {
        return new AdvancedDbCollectionView<TEntityAccess, TEntity>(source, serviceProvider);
    }
}