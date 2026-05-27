//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database;
using kyxsan.Model.Entity;
using kyxsan.Model.Entity.Database;
using kyxsan.Model.Intrinsic;
using kyxsan.Service.Abstraction;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace kyxsan.Service.GachaLog;

[Service(ServiceLifetime.Singleton, typeof(IGachaLogRepository))]
internal sealed partial class GachaLogRepository : IGachaLogRepository
{
    [GeneratedConstructor]
    public partial GachaLogRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public ObservableCollection<GachaArchive> GetGachaArchiveCollection()
    {
        return this.Query(query => query.ToObservableCollection());
    }

    public GachaArchive? GetGachaArchiveByUid(string uid)
    {
        return this.SingleOrDefault(a => a.Uid == uid);
    }

    public GachaArchive? GetGachaArchiveById(Guid archiveId)
    {
        return this.SingleOrDefault(a => a.InnerId == archiveId);
    }

    public void AddGachaArchive(GachaArchive archive)
    {
        this.Add(archive);
    }

    public void DeleteGachaArchiveById(Guid archiveId)
    {
        this.Delete(a => a.InnerId == archiveId);
        DeleteGachaItemsByArchiveId(archiveId);
    }

    public List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId)
    {
        return ExecuteGachaItemQuery(dbSet => dbSet
            .AsNoTracking()
            .Where(i => i.ArchiveId == archiveId)
            .OrderBy(i => i.Id)
            .ToList());
    }

    public ImmutableArray<GachaItem> GetGachaItemImmutableArrayByArchiveId(Guid archiveId)
    {
        return ExecuteGachaItemQuery(dbSet => dbSet
            .AsNoTracking()
            .Where(i => i.ArchiveId == archiveId)
            .OrderBy(i => i.Id)
            .ToImmutableArray());
    }

    public long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType)
    {
        return ExecuteGachaItemQuery(dbSet => dbSet
            .AsNoTracking()
            .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType)
            .OrderByDescending(i => i.Id)
            .Select(i => i.Id)
            .FirstOrDefault());
    }

    public long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType)
    {
        return ExecuteGachaItemQuery(dbSet => dbSet
            .AsNoTracking()
            .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType)
            .OrderBy(i => i.Id)
            .Select(i => i.Id)
            .FirstOrDefault());
    }

    public void AddGachaItemRange(IEnumerable<GachaItem> items)
    {
        ExecuteGachaItemCommand(dbSet => dbSet.AddRangeAndSave(items));
    }

    public void RemoveGachaItemRangeByArchiveId(Guid archiveId)
    {
        DeleteGachaItemsByArchiveId(archiveId);
    }

    public void RemoveGachaItemRangeByArchiveIdAndQueryTypeNewerThanEndId(Guid archiveId, GachaType queryType, long endId)
    {
        ExecuteGachaItemCommand(dbSet => dbSet
            .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType && i.Id > endId)
            .ExecuteDelete());
    }

    private void DeleteGachaItemsByArchiveId(Guid archiveId)
    {
        ExecuteGachaItemCommand(dbSet => dbSet.Where(i => i.ArchiveId == archiveId).ExecuteDelete());
    }

    private TResult ExecuteGachaItemQuery<TResult>(Func<DbSet<GachaItem>, TResult> func)
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return func(dbContext.GachaItems);
    }

    private void ExecuteGachaItemCommand(Func<DbSet<GachaItem>, int> func)
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        func(dbContext.GachaItems);
    }
}
