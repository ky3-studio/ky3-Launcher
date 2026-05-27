//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity.Abstraction;
using System.Collections.Immutable;

namespace kyxsan.Service.Abstraction;

internal static class RepositoryAppDbEntityExtension
{
    extension<TEntity>(IRepository<TEntity> repository)
        where TEntity : class, IAppDbEntity
    {
        public int DeleteByInnerId(TEntity entity)
        {
            return repository.DeleteByInnerId(entity.InnerId);
        }

        public int DeleteByInnerId(Guid innerId)
        {
            return repository.Delete(e => e.InnerId == innerId);
        }
    }

    extension<TEntity>(IRepository<TEntity> repository)
        where TEntity : class, IAppDbEntityHasArchive
    {
        public ImmutableArray<TEntity> ImmutableArrayByArchiveId(Guid archiveId)
        {
            return repository.Query(query => query.Where(e => e.ArchiveId == archiveId).ToImmutableArray());
        }
    }
}