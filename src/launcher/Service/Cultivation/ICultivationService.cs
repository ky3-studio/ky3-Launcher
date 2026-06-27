// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Model.Entity;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Weapon;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.ViewModel.Cultivation;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Launcher.Service.Cultivation;

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

    void BatchAddAllAvatarsAndWeapons(CultivateProject project, IEnumerable<Avatar> avatars, IEnumerable<Weapon> weapons, CultivateLevelInput levelInput);
}
