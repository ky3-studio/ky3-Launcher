// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Entity;
using Launcher.Service.Cultivation;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.UI.Xaml.Data;
using System.Collections.ObjectModel;

namespace Launcher.ViewModel.Cultivation;

internal sealed partial class CultivationViewModel
{
    private async ValueTask UpdateEntryCollectionAsync(CultivateProject? project)
    {
        try
        {
            await taskContext.SwitchToMainThreadAsync();
            EntriesUpdating = true;
            if (project is null)
            {
                return;
            }

            CultivationMetadataContext context = await metadataService.GetContextAsync<CultivationMetadataContext>().ConfigureAwait(false);

            ObservableCollection<CultivateEntryView> entries = await cultivationService
                .GetCultivateEntryCollectionAsync(project, context)
                .ConfigureAwait(false);

            IAdvancedCollectionView<CultivateEntryView> entriesView = entries.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            CultivateEntries = entriesView;

            await UpdateInventoryItemsAsync().ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            EntriesUpdating = false;
        }
    }

    [Command("RemoveEntryCommand")]
    private async Task RemoveEntryAsync(CultivateEntryView? entry)
    {
        if (entry is not null)
        {
            ArgumentNullException.ThrowIfNull(CultivateEntries);
            CultivateEntries.Remove(entry);
            await cultivationService.RemoveCultivateEntryAsync(entry.EntryId).ConfigureAwait(false);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("FinishStateCommand")]
    private async Task UpdateFinishedStateAsync(CultivateItemView? item)
    {
        if (item is not null)
        {
            item.IsFinished = !item.IsFinished;
            cultivationService.SaveCultivateItem(item);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("SaveInventoryItemCommand")]
    private async Task SaveInventoryItemAsync(InventoryItemView? inventoryItem)
    {
        if (inventoryItem is not null)
        {
            cultivationService.SaveInventoryItem(inventoryItem);
            await UpdateStatisticsItemsAsync().ConfigureAwait(false);
        }
    }

    [Command("ClearInventoryCommand")]
    private async Task ClearInventoryAsync(CultivateProject? project)
    {
        if (project is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewModelCultivationClearInventoryTitle, SH.ViewModelCultivationClearInventoryContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        cultivationService.RemoveInventoryItems(project);

        await UpdateInventoryItemsAsync().ConfigureAwait(false);
        await UpdateStatisticsItemsAsync().ConfigureAwait(false);
    }

    [Command("RefreshStatisticsItemsCommand")]
    private async Task UpdateStatisticsItemsAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = null;

        if (Projects?.CurrentItem is null)
        {
            return;
        }

        if (metadataContext is null)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();

        List<StatisticsCultivateItem> statistics = await cultivationService
            .GetStatisticsCultivateItemCollectionAsync(Projects.CurrentItem, metadataContext, CancellationToken)
            .ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        StatisticsItems = new ObservableCollection<StatisticsCultivateItem>(statistics);
        UpdateResinEstimation();
    }

    private void UpdateResinEstimation()
    {
        if (StatisticsItems is null || StatisticsItems.Count == 0)
        {
            ResinEstimationItems = null;
            return;
        }

        ResinEstimationItems = ResinEstimator.Estimate(StatisticsItems, WorldLevel);
    }

    private async ValueTask UpdateInventoryItemsAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = [];

        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        InventoryItems = cultivationService.GetInventoryItemViews(metadataContext, Projects.CurrentItem);
    }
}
