// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database;
using kyxsan.Model.Entity;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.ViewModel.Cultivation;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace kyxsan.Service.Cultivation;

internal interface ICultivationService
{
    ValueTask<IAdvancedDbCollectionView<CultivateProject>> GetProjectCollectionAsync();

    ValueTask<bool> EnsureCurrentProjectAsync(IAdvancedDbCollectionView<CultivateProject> projects);

    ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntryCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context);

    ValueTask<List<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token);

    ValueTask RemoveCultivateEntryAsync(Guid entryId);

    ValueTask RemoveProjectAsync(CultivateProject project);

    void SaveCultivateItem(CultivateItemView item);

    ValueTask<ProjectAddResultKind> TryAddProjectAsync(CultivateProject project);

    void SaveInventoryItem(InventoryItemView item);

    ImmutableArray<InventoryItemView> GetInventoryItemViews(ICultivationMetadataContext context, CultivateProject project);

    void RemoveInventoryItems(CultivateProject project);

    void RefreshInventoryFromYae(CultivateProject project, ImmutableArray<(uint ItemId, uint Count)> storeItems);

    void AddCultivateEntryFromAvatar(CultivateProject project, Avatar avatar, CultivateLevelInput levelInput);

    void AddCultivateEntryFromWeapon(CultivateProject project, Weapon weapon, uint levelFrom, uint levelTo, bool ascended);
}
