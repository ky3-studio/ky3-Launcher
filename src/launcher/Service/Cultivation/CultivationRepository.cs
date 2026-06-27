// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Launcher.Model.Entity;
using Launcher.Service.Abstraction;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Launcher.Service.Cultivation;

[Service(ServiceLifetime.Singleton, typeof(ICultivationRepository))]
internal sealed partial class CultivationRepository : ICultivationRepository
{
    [GeneratedConstructor]
    public partial CultivationRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public ObservableCollection<CultivateProject> GetCultivateProjectCollection()
    {
        return ((IRepository<CultivateProject>)this).ObservableCollection();
    }

    public ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayByProjectId(Guid projectId)
    {
        return ((IRepository<CultivateEntry>)this).ImmutableArray(e => e.ProjectId == projectId);
    }

    public ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayIncludingLevelInformationByProjectId(Guid projectId)
    {
        return ((IRepository<CultivateEntry>)this).ImmutableArray(query => query.Where(e => e.ProjectId == projectId).Include(e => e.LevelInformation));
    }

    public ImmutableArray<CultivateItem> GetCultivateItemImmutableArrayByEntryId(Guid entryId)
    {
        return ((IRepository<CultivateItem>)this).ImmutableArray(query => query.Where(i => i.EntryId == entryId).OrderBy(i => i.ItemId));
    }

    public ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayByProjectIdAndItemId(Guid projectId, uint itemId)
    {
        return ((IRepository<CultivateEntry>)this).ImmutableArray(e => e.ProjectId == projectId && e.Id == itemId);
    }

    public void AddCultivateEntry(CultivateEntry entry)
    {
        ((IRepository<CultivateEntry>)this).Add(entry);
    }

    public void AddCultivateEntryRange(IEnumerable<CultivateEntry> entries)
    {
        ((IRepository<CultivateEntry>)this).AddRange(entries);
    }

    public void AddLevelInformationRange(IEnumerable<CultivateEntryLevelInformation> levelInformations)
    {
        ((IRepository<CultivateEntryLevelInformation>)this).AddRange(levelInformations);
    }

    public void RemoveCultivateEntryById(Guid entryId)
    {
        ((IRepository<CultivateEntry>)this).DeleteByInnerId(entryId);
    }

    public void RemoveCultivateItemRangeByEntryId(Guid entryId)
    {
        ((IRepository<CultivateItem>)this).Delete(i => i.EntryId == entryId);
    }

    public void AddCultivateItemRange(IEnumerable<CultivateItem> toAdd)
    {
        ((IRepository<CultivateItem>)this).AddRange(toAdd);
    }

    public void UpdateCultivateItem(CultivateItem item)
    {
        ((IRepository<CultivateItem>)this).Update(item);
    }

    public void AddLevelInformation(CultivateEntryLevelInformation levelInformation)
    {
        ((IRepository<CultivateEntryLevelInformation>)this).Add(levelInformation);
    }

    public void RemoveLevelInformationByEntryId(Guid entryId)
    {
        ((IRepository<CultivateEntryLevelInformation>)this).Delete(l => l.EntryId == entryId);
    }

    public void RemoveCultivateProjectById(Guid projectId)
    {
        ((IRepository<CultivateProject>)this).DeleteByInnerId(projectId);
    }

    public ImmutableArray<InventoryItem> GetInventoryItemImmutableArrayByProjectId(Guid projectId)
    {
        return ((IRepository<InventoryItem>)this).ImmutableArray(i => i.ProjectId == projectId);
    }

    public void SaveInventoryItems(Guid projectId, IEnumerable<InventoryItem> items)
    {
        ((IRepository<InventoryItem>)this).Delete(i => i.ProjectId == projectId);
        ((IRepository<InventoryItem>)this).AddRange(items);
    }

    public void RemoveInventoryItemsByProjectId(Guid projectId)
    {
        ((IRepository<InventoryItem>)this).Delete(i => i.ProjectId == projectId);
    }

    public void Update(CultivateProject project)
    {
        ((IRepository<CultivateProject>)this).Update(project);
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        ((IRepository<InventoryItem>)this).Update(item);
    }

    public void AddInventoryItem(InventoryItem item)
    {
        ((IRepository<InventoryItem>)this).Add(item);
    }
}
