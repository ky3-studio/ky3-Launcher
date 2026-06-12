// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Service.Abstraction;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace kyxsan.Service.Cultivation;

internal interface ICultivationRepository :
    IRepository<CultivateProject>,
    IRepository<CultivateEntry>,
    IRepository<CultivateItem>,
    IRepository<CultivateEntryLevelInformation>,
    IRepository<InventoryItem>
{
    ObservableCollection<CultivateProject> GetCultivateProjectCollection();

    ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayByProjectId(Guid projectId);

    ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayIncludingLevelInformationByProjectId(Guid projectId);

    ImmutableArray<CultivateItem> GetCultivateItemImmutableArrayByEntryId(Guid entryId);

    ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayByProjectIdAndItemId(Guid projectId, uint itemId);

    void AddCultivateEntry(CultivateEntry entry);

    void RemoveCultivateEntryById(Guid entryId);

    void RemoveCultivateItemRangeByEntryId(Guid entryId);

    void AddCultivateItemRange(IEnumerable<CultivateItem> toAdd);

    void UpdateCultivateItem(CultivateItem item);

    void AddLevelInformation(CultivateEntryLevelInformation levelInformation);

    void RemoveLevelInformationByEntryId(Guid entryId);

    void RemoveCultivateProjectById(Guid projectId);

    ImmutableArray<InventoryItem> GetInventoryItemImmutableArrayByProjectId(Guid projectId);

    void SaveInventoryItems(Guid projectId, IEnumerable<InventoryItem> items);

    void RemoveInventoryItemsByProjectId(Guid projectId);

    void UpdateInventoryItem(InventoryItem item);

    void AddInventoryItem(InventoryItem item);

    void Update(CultivateProject project);
}
