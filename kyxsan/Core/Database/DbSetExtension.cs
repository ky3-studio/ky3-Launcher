//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.Database;

internal static class DbSetExtension
{
    extension<TEntity>(DbSet<TEntity> dbSet)
        where TEntity : class
    {
        public int AddAndSave(TEntity entity)
        {
            dbSet.Add(entity);
            return dbSet.SaveChangesAndClearChangeTracker();
        }

        public int AddRangeAndSave(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
            return dbSet.SaveChangesAndClearChangeTracker();
        }

        public int RemoveAndSave(TEntity entity)
        {
            dbSet.Remove(entity);
            return dbSet.SaveChangesAndClearChangeTracker();
        }

        public int UpdateAndSave(TEntity entity)
        {
            dbSet.Update(entity);
            return dbSet.SaveChangesAndClearChangeTracker();
        }

        public int UpdateRangeAndSave(IEnumerable<TEntity> entity)
        {
            dbSet.UpdateRange(entity);
            return dbSet.SaveChangesAndClearChangeTracker();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SaveChangesAndClearChangeTracker()
        {
            DbContext dbContext = dbSet.Context();
            int count = dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DbContext Context()
        {
            return dbSet.GetService<ICurrentDbContext>().Context;
        }
    }
}