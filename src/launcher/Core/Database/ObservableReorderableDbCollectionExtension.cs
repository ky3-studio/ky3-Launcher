//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database.Abstraction;
using kyxsan.Model;
using kyxsan.UI.Xaml.Data;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.Database;

internal static class ObservableReorderableDbCollectionExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntity> ToObservableReorderableDbCollection<TEntity>(this IEnumerable<TEntity> source, IServiceProvider serviceProvider)
        where TEntity : class, IReorderable
    {
        return source is List<TEntity> list
            ? new(list, serviceProvider)
            : new([.. source], serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntityOnly, TEntity> ToObservableReorderableDbCollection<TEntityOnly, TEntity>(this IEnumerable<TEntityOnly> source, IServiceProvider serviceProvider)
        where TEntityOnly : class, IEntityAccess<TEntity>
        where TEntity : class, IReorderable
    {
        return source is List<TEntityOnly> list
            ? new(list, serviceProvider)
            : new([.. source], serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdvancedDbCollectionView<TEntityOnly, TEntity> ToAdvancedDbCollectionViewWrappedObservableReorderableDbCollection<TEntityOnly, TEntity>(this IEnumerable<TEntityOnly> source, IServiceProvider serviceProvider)
        where TEntityOnly : class, IPropertyValuesProvider, IEntityAccess<TEntity>
        where TEntity : class, ISelectable, IReorderable
    {
        return new(ToObservableReorderableDbCollection<TEntityOnly, TEntity>(source, serviceProvider), serviceProvider);
    }
}