//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace kyxsan.Service.Abstraction;

internal static class RepositoryCollectionExtension
{
    extension<TEntity>(IRepository<TEntity> repository)
        where TEntity : class
    {
        public ImmutableArray<TEntity> ImmutableArray()
        {
            return repository.Query(query => query.ToImmutableArray());
        }

        public ImmutableArray<TEntity> ImmutableArray(Expression<Func<TEntity, bool>> predicate)
        {
            return repository.ImmutableArray(query => query.Where(predicate));
        }

        public ImmutableArray<TResult> ImmutableArray<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            return repository.Query(query1 => query(query1).ToImmutableArray());
        }

        // ObservableCollection<TEntity> is always not readonly.
        public ObservableCollection<TEntity> ObservableCollection()
        {
            return repository.Query(query => query.ToObservableCollection());
        }

        public ObservableCollection<TEntity> ObservableCollection(Expression<Func<TEntity, bool>> predicate)
        {
            return repository.Query(query => query.Where(predicate).ToObservableCollection());
        }
    }
}