//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using kyxsan.Core.Database;
using kyxsan.Model.Entity.Database;
using System.Linq.Expressions;

namespace kyxsan.Service.Abstraction;

internal static class RepositoryExtension
{
    extension<TEntity>(IRepository<TEntity> repository)
        where TEntity : class
    {
        public TResult Execute<TResult>(Func<DbSet<TEntity>, TResult> func)
        {
            using (IServiceScope scope = repository.ServiceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.GetAppDbContext();
                return func(appDbContext.Set<TEntity>());
            }
        }

        public int Add(TEntity entity)
        {
            return repository.Execute(dbSet => dbSet.AddAndSave(entity));
        }

        public int AddRange(IEnumerable<TEntity> entities)
        {
            return repository.Execute(dbSet => dbSet.AddRangeAndSave(entities));
        }

        public int Delete()
        {
            return repository.Execute(dbSet => dbSet.ExecuteDelete());
        }

        public int Delete(TEntity entity)
        {
            return repository.Execute(dbSet => dbSet.RemoveAndSave(entity));
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            return repository.Execute(dbSet => dbSet.Where(predicate).ExecuteDelete());
        }

        public TResult Query<TResult>(Func<IQueryable<TEntity>, TResult> func)
        {
            return repository.Execute(dbSet => func(dbSet.AsNoTracking()));
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return repository.Query(query => query.Single(predicate));
        }

        public TResult Single<TResult>(Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        {
            return repository.Query(query1 => query(query1).Single());
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return repository.Query(query => query.SingleOrDefault(predicate));
        }

        public int Update(TEntity entity)
        {
            return repository.Execute(dbSet => dbSet.UpdateAndSave(entity));
        }
    }
}