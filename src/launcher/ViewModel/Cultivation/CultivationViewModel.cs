// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core;
using kyxsan.Core.Database;
using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Factory.ContentDialog;
using kyxsan.Model.Entity;
using kyxsan.Model.Primitive;
using kyxsan.Service.Cultivation;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Notification;
using kyxsan.Service.User;
using kyxsan.Service.Yae;
using kyxsan.UI.Xaml.Data;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using kyxsan.ViewModel.AvatarProperty;
using kyxsan.Web.Hoyolab.Takumi.GameRecord;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;
using kyxsan.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using MetaAvatar = kyxsan.Model.Metadata.Avatar.Avatar;
using MetaWeapon = kyxsan.Model.Metadata.Weapon.Weapon;

namespace kyxsan.ViewModel.Cultivation;

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

    [Command("RefreshInventoryByEmbeddedYaeCommand")]
    private async Task RefreshInventoryByEmbeddedYaeAsync()
    {
        if (Projects?.CurrentItem is null)
        {
            return;
        }

        if (!kyxsanRuntime.IsProcessElevated)
        {
            messenger.Send(InfoBarMessage.Error(SH.ServiceGameLaunchingHandlerEmbeddedYaeClientNotElevated));
            return;
        }

        ImmutableArray<(uint ItemId, uint Count)>? items = await yaeService.GetInventoryAsync().ConfigureAwait(false);

        if (items is not { } storeItems || storeItems.IsEmpty)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();

        if (SyncInventoryToAllProjects && Projects.Source is { Count: > 0 } allProjects)
        {
            foreach (CultivateProject project in allProjects)
            {
                cultivationService.RefreshInventoryFromYae(project, storeItems);
            }
        }
        else
        {
            cultivationService.RefreshInventoryFromYae(Projects.CurrentItem, storeItems);
        }

        await UpdateInventoryItemsAsync().ConfigureAwait(false);
        await UpdateStatisticsItemsAsync().ConfigureAwait(false);
    }

    [Command("SyncAllAvatarsAndWeaponsCommand")]
    private async Task SyncAllAvatarsAndWeaponsAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

        Response<ListWrapper<Character>> response = await gameRecordClient
            .GetCharacterListAsync(userAndUid)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(response, serviceProvider, out ListWrapper<Character>? wrapper))
        {
            return;
        }

        List<CharacterView> characters = [];
        foreach (Character apiChar in wrapper.List)
        {
            if (metadataContext.IdAvatarMap.TryGetValue(apiChar.Id, out MetaAvatar? metaAvatar))
            {
                metadataContext.IdWeaponMap.TryGetValue(apiChar.Weapon.Id, out MetaWeapon? metaWeapon);
                characters.Add(new CharacterView(apiChar, metaAvatar, metaWeapon));
            }
        }

        Response<ListWrapper<DetailedCharacter>> detailResponse = await gameRecordClient
            .GetCharacterDetailAsync(userAndUid, wrapper.List.SelectAsArray(static c => c.Id))
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(detailResponse, serviceProvider, out ListWrapper<DetailedCharacter>? detailWrapper))
        {
            foreach (DetailedCharacter detail in detailWrapper.List)
            {
                CharacterView? characterView = characters.Find(c => c.Id == detail.Base.Id);
                if (characterView is not null && metadataContext.IdAvatarMap.TryGetValue(detail.Base.Id, out MetaAvatar? detailMetaAvatar))
                {
                    characterView.SetSkills(detail.Skills, detailMetaAvatar);
                }
            }
        }

        await taskContext.SwitchToBackgroundAsync();

        int addedCount = 0;
        foreach (CharacterView character in characters)
        {
            if (!metadataContext.IdAvatarMap.TryGetValue(character.Id, out MetaAvatar? metaAvatar))
            {
                continue;
            }

            uint avatarLevelFrom = character.LevelNumber;
            uint skillAFrom = character.Skills.Length > 0 ? (uint)character.Skills[0].Level : 1;
            uint skillEFrom = character.Skills.Length > 1 ? (uint)character.Skills[1].Level : 1;
            uint skillQFrom = character.Skills.Length > 2 ? (uint)character.Skills[2].Level : 1;

            CultivateLevelInput levelInput = new()
            {
                AvatarLevelFrom = avatarLevelFrom,
                AvatarLevelTo = 90,
                SkillALevelFrom = skillAFrom,
                SkillALevelTo = 10,
                SkillELevelFrom = skillEFrom,
                SkillELevelTo = 10,
                SkillQLevelFrom = skillQFrom,
                SkillQLevelTo = 10,
                WeaponLevelFrom = character.WeaponLevelNumber,
                WeaponLevelTo = 90,
                WeaponAscended = false,
            };

            cultivationService.AddCultivateEntryFromAvatar(Projects.CurrentItem, metaAvatar, levelInput);

            if (metadataContext.IdWeaponMap.TryGetValue(character.WeaponId, out MetaWeapon? metaWeapon)
                && levelInput.WeaponLevelTo > levelInput.WeaponLevelFrom)
            {
                cultivationService.AddCultivateEntryFromWeapon(Projects.CurrentItem, metaWeapon, levelInput.WeaponLevelFrom, levelInput.WeaponLevelTo, false);
            }

            addedCount++;
        }

        messenger.Send(InfoBarMessage.Success(SH.FormatViewModelCultivationBatchAddCompleted(addedCount, 0)));
        await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
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

    [Command("NavigateToPageCommand")]
    private async Task OpenAvatarSelectionAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        CultivateAvatarDialog avatarDialog = await contentDialogFactory.CreateInstanceAsync<CultivateAvatarDialog>(serviceProvider).ConfigureAwait(false);
        ImmutableArray<MetaAvatar> avatars = [.. metadataContext.Avatars.OrderByDescending(a => a.BeginTime).ThenByDescending(a => a.Sort)];
        (bool isOk, ImmutableArray<MetaAvatar> selectedAvatars) = await avatarDialog.SelectAvatarsAsync(avatars).ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        ImmutableArray<(MetaAvatar, CharacterView?, MetaWeapon?)> entries = [.. selectedAvatars.Select(a => (a, (CharacterView?)null, (MetaWeapon?)null))];

        CultivateLevelDialog levelDialog = await contentDialogFactory.CreateInstanceAsync<CultivateLevelDialog>(serviceProvider).ConfigureAwait(false);
        (bool levelOk, ImmutableArray<CultivateAvatarLevelItem> levelItems) = await levelDialog.SelectLevelsAsync(entries).ConfigureAwait(false);

        if (!levelOk)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        foreach (CultivateAvatarLevelItem item in levelItems)
        {
            CultivateLevelInput levelInput = item.ToLevelInput();
            cultivationService.AddCultivateEntryFromAvatar(Projects.CurrentItem, item.Avatar, levelInput);
        }

        await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
    }

    [Command("NavigateToWeaponPageCommand")]
    private async Task OpenWeaponSelectionAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        CultivateWeaponDialog weaponDialog = await contentDialogFactory.CreateInstanceAsync<CultivateWeaponDialog>(serviceProvider).ConfigureAwait(false);
        ImmutableArray<MetaWeapon> weapons = [.. metadataContext.IdWeaponMap.Values.OrderByDescending(w => w.Sort)];
        (bool isOk, ImmutableArray<MetaWeapon> selectedWeapons) = await weaponDialog.SelectWeaponsAsync(weapons).ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        CultivateWeaponLevelDialog levelDialog = await contentDialogFactory.CreateInstanceAsync<CultivateWeaponLevelDialog>(serviceProvider).ConfigureAwait(false);
        (bool levelOk, ImmutableArray<CultivateWeaponLevelItem> levelItems) = await levelDialog.SelectLevelsAsync(selectedWeapons).ConfigureAwait(false);

        if (!levelOk)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        foreach (CultivateWeaponLevelItem item in levelItems)
        {
            cultivationService.AddCultivateEntryFromWeapon(Projects.CurrentItem, item.Weapon, (uint)item.LevelFrom, (uint)item.LevelTo, false);
        }

        await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
    }

    [Command("MyAvatarCultivateCommand")]
    private async Task MyAvatarCultivateAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

        Response<ListWrapper<Character>> response = await gameRecordClient
            .GetCharacterListAsync(userAndUid)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(response, serviceProvider, out ListWrapper<Character>? wrapper))
        {
            return;
        }

        List<CharacterView> characters = [];
        foreach (Character apiChar in wrapper.List)
        {
            if (metadataContext.IdAvatarMap.TryGetValue(apiChar.Id, out MetaAvatar? metaAvatar))
            {
                metadataContext.IdWeaponMap.TryGetValue(apiChar.Weapon.Id, out MetaWeapon? metaWeapon);
                characters.Add(new CharacterView(apiChar, metaAvatar, metaWeapon));
            }
        }

        Response<ListWrapper<DetailedCharacter>> detailResponse = await gameRecordClient
            .GetCharacterDetailAsync(userAndUid, wrapper.List.SelectAsArray(static c => c.Id))
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(detailResponse, serviceProvider, out ListWrapper<DetailedCharacter>? detailWrapper))
        {
            foreach (DetailedCharacter detail in detailWrapper.List)
            {
                CharacterView? characterView = characters.Find(c => c.Id == detail.Base.Id);
                if (characterView is not null && metadataContext.IdAvatarMap.TryGetValue(detail.Base.Id, out MetaAvatar? detailMetaAvatar))
                {
                    characterView.SetSkills(detail.Skills, detailMetaAvatar);
                }
            }
        }

        // Filter metadata avatars to only user-owned ones
        HashSet<AvatarId> ownedIds = [.. characters.Select(c => c.Id)];
        ImmutableArray<MetaAvatar> ownedAvatars = [.. metadataContext.Avatars
            .Where(a => ownedIds.Contains(a.Id))
            .OrderByDescending(a => a.BeginTime)
            .ThenByDescending(a => a.Sort)];

        // Create avatar dialog upfront while serviceProvider is still alive
        CultivateAvatarDialog avatarDialog = await contentDialogFactory.CreateInstanceAsync<CultivateAvatarDialog>(serviceProvider).ConfigureAwait(false);

        (bool isOk, ImmutableArray<MetaAvatar> selectedAvatars) = await avatarDialog.SelectAvatarsAsync(ownedAvatars).ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        ImmutableArray<(MetaAvatar, CharacterView?, MetaWeapon?)> entries = [.. selectedAvatars.Select(selectedAvatar =>
        {
            CharacterView? selectedCharView = characters.Find(c => (uint)c.Id == (uint)selectedAvatar.Id);
            MetaWeapon? selectedWeapon = null;
            if (selectedCharView is not null)
            {
                metadataContext.IdWeaponMap.TryGetValue(selectedCharView.WeaponId, out selectedWeapon);
            }

            return (selectedAvatar, selectedCharView, selectedWeapon);
        })];

        CultivateLevelDialog levelDialog = await contentDialogFactory.CreateInstanceAsync<CultivateLevelDialog>(serviceProvider).ConfigureAwait(false);
        (bool levelOk, ImmutableArray<CultivateAvatarLevelItem> levelItems) = await levelDialog.SelectLevelsAsync(entries).ConfigureAwait(false);

        if (!levelOk)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        foreach (CultivateAvatarLevelItem item in levelItems)
        {
            CultivateLevelInput levelInput = item.ToLevelInput();
            cultivationService.AddCultivateEntryFromAvatar(Projects.CurrentItem, item.Avatar, levelInput);

            if (item.Weapon is not null && levelInput.WeaponLevelTo > levelInput.WeaponLevelFrom)
            {
                cultivationService.AddCultivateEntryFromWeapon(Projects.CurrentItem, item.Weapon, levelInput.WeaponLevelFrom, levelInput.WeaponLevelTo, levelInput.WeaponAscended);
            }
        }

        await UpdateEntryCollectionAsync(Projects.CurrentItem).ConfigureAwait(false);
    }
}
