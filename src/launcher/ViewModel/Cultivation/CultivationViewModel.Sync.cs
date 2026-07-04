// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Entity;
using Launcher.Model.Primitive;
using Launcher.Service.Cultivation;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Notification;
using Launcher.Service.User;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.ViewModel.AvatarProperty;
using Launcher.Web.Hoyolab.Takumi.GameRecord;
using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Launcher.Web.Response;
using System.Collections.Immutable;
using MetaAvatar = Launcher.Model.Metadata.Avatar.Avatar;
using MetaWeapon = Launcher.Model.Metadata.Weapon.Weapon;

namespace Launcher.ViewModel.Cultivation;

internal sealed partial class CultivationViewModel
{
    [Command("RefreshInventoryByEmbeddedYaeCommand")]
    private async Task RefreshInventoryByEmbeddedYaeAsync()
    {
        if (Projects?.CurrentItem is null)
        {
            return;
        }

        if (!LauncherRuntime.IsProcessElevated)
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

    [Command("SyncAllAvatarsCommand")]
    private async Task SyncAllAvatarsAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        CultivateProject currentProject = Projects.CurrentItem;

        await taskContext.SwitchToMainThreadAsync();
        EntriesUpdating = true;

        await taskContext.SwitchToBackgroundAsync();

        CultivateLevelInput levelInput = new()
        {
            AvatarLevelFrom = 1,
            AvatarLevelTo = 90,
            SkillALevelFrom = 1,
            SkillALevelTo = 10,
            SkillELevelFrom = 1,
            SkillELevelTo = 10,
            SkillQLevelFrom = 1,
            SkillQLevelTo = 10,
            WeaponLevelFrom = 1,
            WeaponLevelTo = 90,
            WeaponAscended = false,
        };

        cultivationService.BatchAddAllAvatarsAndWeapons(
            currentProject,
            metadataContext.Avatars,
            [],
            levelInput);

        int addedCount = metadataContext.Avatars.Length;
        messenger.Send(InfoBarMessage.Success(SH.FormatViewModelCultivationBatchAddCompleted(addedCount, 0)));
        await UpdateEntryCollectionAsync(currentProject).ConfigureAwait(false);
    }

    [Command("SyncAllWeaponsCommand")]
    private async Task SyncAllWeaponsAsync()
    {
        if (Projects?.CurrentItem is null || metadataContext is null)
        {
            return;
        }

        CultivateProject currentProject = Projects.CurrentItem;

        await taskContext.SwitchToMainThreadAsync();
        EntriesUpdating = true;

        await taskContext.SwitchToBackgroundAsync();

        CultivateLevelInput levelInput = new()
        {
            AvatarLevelFrom = 1,
            AvatarLevelTo = 90,
            SkillALevelFrom = 1,
            SkillALevelTo = 10,
            SkillELevelFrom = 1,
            SkillELevelTo = 10,
            SkillQLevelFrom = 1,
            SkillQLevelTo = 10,
            WeaponLevelFrom = 1,
            WeaponLevelTo = 90,
            WeaponAscended = false,
        };

        cultivationService.BatchAddAllAvatarsAndWeapons(
            currentProject,
            [],
            metadataContext.IdWeaponMap.Values,
            levelInput);

        int addedCount = metadataContext.IdWeaponMap.Count;
        messenger.Send(InfoBarMessage.Success(SH.FormatViewModelCultivationBatchAddCompleted(addedCount, 0)));
        await UpdateEntryCollectionAsync(currentProject).ConfigureAwait(false);
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

        HashSet<AvatarId> ownedIds = [.. characters.Select(c => c.Id)];
        ImmutableArray<MetaAvatar> ownedAvatars = [.. metadataContext.Avatars
            .Where(a => ownedIds.Contains(a.Id))
            .OrderByDescending(a => a.BeginTime)
            .ThenByDescending(a => a.Sort)];

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
