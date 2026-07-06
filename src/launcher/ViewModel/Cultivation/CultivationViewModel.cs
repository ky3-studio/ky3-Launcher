// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Launcher.Core.Database;
using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Entity;
using Launcher.Service.Cultivation;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.User;
using Launcher.Service.Yae;
using Launcher.UI.Xaml.Data;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.UI.Xaml.Control.AutoSuggestBox;
using Launcher.Web.Hoyolab.Takumi.GameRecord;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Launcher.ViewModel.Cultivation;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class CultivationViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ICultivationService cultivationService;
    private readonly IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IYaeService yaeService;
    private readonly IMessenger messenger;

    private CultivationMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial CultivationViewModel(IServiceProvider serviceProvider);

    public IAdvancedDbCollectionView<CultivateProject>? Projects
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentProjectChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentProjectChanged);
        }
    }

    [ObservableProperty]
    public partial ImmutableArray<InventoryItemView> InventoryItems { get; set; } = [];

    [ObservableProperty]
    public partial IAdvancedCollectionView<CultivateEntryView>? CultivateEntries { get; set; }

    [ObservableProperty]
    public partial bool EntriesUpdating { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<StatisticsCultivateItem>? StatisticsItems { get; set; }

    [ObservableProperty]
    public partial int WorldLevel { get; set; } = 8;

    [ObservableProperty]
    public partial List<ResinEstimationItem>? ResinEstimationItems { get; set; }

    [ObservableProperty]
    public partial bool SyncInventoryToAllProjects { get; set; }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    partial void OnWorldLevelChanged(int value)
    {
        UpdateResinEstimation();
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        if (CultivateEntries is null || metadataContext is null)
        {
            return;
        }

        CultivateEntries.Filter = CultivateEntryViewFilter.Compile(SearchData, metadataContext);
        CultivateEntries.Refresh();
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<CultivationMetadataContext>(token).ConfigureAwait(false);

        SearchData searchData = SearchData.CreateForCultivation();
        await taskContext.SwitchToMainThreadAsync();
        SearchData = searchData;
        await taskContext.SwitchToBackgroundAsync();

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            IAdvancedDbCollectionView<CultivateProject> projects = await cultivationService.GetProjectCollectionAsync().ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Projects = projects;
            Projects.MoveCurrentTo(Projects.Source.SelectedOrFirstOrDefault());
        }

        if (Projects.CurrentItem is not null && CultivateEntries is null)
        {
            await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
        }

        return true;
    }

    protected override void UninitializeOverride()
    {
        using (Projects?.SuppressChangeCurrentItem())
        {
            Projects = default;
        }
    }

    private void OnCurrentProjectChanged(object? sender, object? e)
    {
        UpdateEntryCollectionAsync(Projects?.CurrentItem).SafeForget();
    }

    [Command("AddProjectCommand")]
    private async Task AddProjectAsync()
    {
        CultivateProjectDialog dialog = await contentDialogFactory.CreateInstanceAsync<CultivateProjectDialog>(serviceProvider).ConfigureAwait(false);
        (bool isOk, CultivateProject project) = await dialog.CreateProjectAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        await cultivationService.TryAddProjectAsync(project).ConfigureAwait(false);
    }

    [Command("RemoveProjectCommand")]
    private async Task RemoveProjectAsync(CultivateProject? project)
    {
        if (project is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewModelCultivationRemoveProjectTitle, SH.ViewModelCultivationRemoveProjectContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        await cultivationService.RemoveProjectAsync(project).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        Projects?.MoveCurrentToFirst();
    }
}
