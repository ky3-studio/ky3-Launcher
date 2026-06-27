// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Model.Entity;
using Launcher.Model.Entity.Primitive;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Item;
using Launcher.Model.Metadata.Weapon;
using Launcher.Service.Cultivation.Offline;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.ViewModel.Cultivation;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Launcher.Service.Cultivation;

[Service(ServiceLifetime.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly AsyncLock projectsLock = new();

    private readonly ICultivationRepository cultivationRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial CultivationService(IServiceProvider serviceProvider);

    private AdvancedDbCollectionView<CultivateProject>? projects;

    public async ValueTask<IAdvancedDbCollectionView<CultivateProject>> GetProjectCollectionAsync()
    {
        using (await projectsLock.LockAsync().ConfigureAwait(false))
        {
            return projects ??= new(cultivationRepository.GetCultivateProjectCollection(), serviceProvider);
        }
    }

    public async ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntryCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context)
    {
        await taskContext.SwitchToBackgroundAsync();

        ImmutableArray<CultivateEntry> entries = cultivationRepository.GetCultivateEntryImmutableArrayIncludingLevelInformationByProjectId(cultivateProject.InnerId);

        List<CultivateEntryView> resultEntries = new(entries.Length);
        foreach (CultivateEntry entry in entries)
        {
            ImmutableArray<CultivateItem> items = cultivationRepository.GetCultivateItemImmutableArrayByEntryId(entry.InnerId);
            if (items.Length == 0 && entry.Type is CultivateType.AvatarAndSkill)
            {
                continue;
            }

            ImmutableArray<CultivateItemView>.Builder entryItems = ImmutableArray.CreateBuilder<CultivateItemView>(items.Length);
            foreach (CultivateItem cultivateItem in items)
            {
                Material material = context.GetMaterial(cultivateItem.ItemId);
                entryItems.Add(CultivateItemView.Create(cultivateItem, material, cultivateProject.ServerTimeZoneOffset));
            }

            resultEntries.Add(CultivateEntryView.Create(entry, entryItems.ToImmutable(), context, cultivateProject.ServerTimeZoneOffset));
        }

        resultEntries.Sort((a, b) => b.IsToday.CompareTo(a.IsToday));
        return new ObservableCollection<CultivateEntryView>(resultEntries);
    }

    public async ValueTask<List<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await taskContext.SwitchToBackgroundAsync();
        token.ThrowIfCancellationRequested();

        Dictionary<uint, StatisticsCultivateItem> resultItems = [];
        Guid projectId = cultivateProject.InnerId;

        foreach (CultivateEntry entry in cultivationRepository.GetCultivateEntryImmutableArrayByProjectId(projectId))
        {
            token.ThrowIfCancellationRequested();
            foreach (CultivateItem item in cultivationRepository.GetCultivateItemImmutableArrayByEntryId(entry.InnerId))
            {
                token.ThrowIfCancellationRequested();
                if (item.IsFinished)
                {
                    continue;
                }

                ref StatisticsCultivateItem? existedItem = ref CollectionsMarshal.GetValueRefOrAddDefault(resultItems, item.ItemId, out _);
                if (existedItem is null)
                {
                    existedItem = StatisticsCultivateItem.Create(context.GetMaterial(item.ItemId), item.Count, cultivateProject.ServerTimeZoneOffset);
                }
                else
                {
                    existedItem.Count += item.Count;
                }
            }
        }

        // Apply inventory
        foreach (InventoryItem inventoryItem in cultivationRepository.GetInventoryItemImmutableArrayByProjectId(projectId))
        {
            token.ThrowIfCancellationRequested();
            ref StatisticsCultivateItem existedItem = ref CollectionsMarshal.GetValueRefOrNullRef(resultItems, inventoryItem.ItemId);
            if (!System.Runtime.CompilerServices.Unsafe.IsNullRef(in existedItem))
            {
                existedItem.Current = inventoryItem.Count;
            }
        }

        List<StatisticsCultivateItem> result = [.. resultItems.Values];
        result.Sort((a, b) =>
        {
            int cmp = a.IsFinished.CompareTo(b.IsFinished);
            if (cmp != 0) return cmp;
            cmp = b.IsToday.CompareTo(a.IsToday);
            return cmp;
        });

        return result;
    }

    public async ValueTask RemoveCultivateEntryAsync(Guid entryId)
    {
        await taskContext.SwitchToBackgroundAsync();
        cultivationRepository.RemoveLevelInformationByEntryId(entryId);
        cultivationRepository.RemoveCultivateItemRangeByEntryId(entryId);
        cultivationRepository.RemoveCultivateEntryById(entryId);
    }

    public void SaveCultivateItem(CultivateItemView item)
    {
        cultivationRepository.UpdateCultivateItem(item.Entity);
    }

    public async ValueTask<ProjectAddResultKind> TryAddProjectAsync(CultivateProject project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            return ProjectAddResultKind.InvalidName;
        }

        ArgumentNullException.ThrowIfNull(projects);

        if (projects.Source.Any(a => a.Name == project.Name))
        {
            return ProjectAddResultKind.AlreadyExists;
        }

        await taskContext.SwitchToMainThreadAsync();
        projects.Add(project);
        projects.MoveCurrentTo(project);

        return ProjectAddResultKind.Added;
    }

    public async ValueTask RemoveProjectAsync(CultivateProject project)
    {
        ArgumentNullException.ThrowIfNull(projects);

        await taskContext.SwitchToMainThreadAsync();
        projects.Remove(project);

        await taskContext.SwitchToBackgroundAsync();
        cultivationRepository.RemoveCultivateProjectById(project.InnerId);
    }

    public async ValueTask<bool> EnsureCurrentProjectAsync(IAdvancedDbCollectionView<CultivateProject> projects)
    {
        if (projects.CurrentItem is null)
        {
            try
            {
                await taskContext.SwitchToMainThreadAsync();
                projects.MoveCurrentTo(projects.Source.SelectedOrFirstOrDefault());
            }
            catch (InvalidOperationException)
            {
            }

            if (projects.CurrentItem is null)
            {
                return false;
            }
        }

        return true;
    }

    public void SaveInventoryItem(InventoryItemView item)
    {
        if (item.Entity.InnerId == default)
        {
            cultivationRepository.AddInventoryItem(item.Entity);
        }
        else
        {
            cultivationRepository.UpdateInventoryItem(item.Entity);
        }
    }

    public ImmutableArray<InventoryItemView> GetInventoryItemViews(ICultivationMetadataContext context, CultivateProject project)
    {
        Guid projectId = project.InnerId;
        ImmutableArray<InventoryItem> existingItems = cultivationRepository.GetInventoryItemImmutableArrayByProjectId(projectId);
        Dictionary<uint, InventoryItem> entityMap = existingItems.ToDictionary(i => i.ItemId);

        ImmutableArray<InventoryItemView>.Builder result = ImmutableArray.CreateBuilder<InventoryItemView>();
        foreach (Material material in context.EnumerateInventoryMaterial())
        {
            InventoryItem entity = entityMap.GetValueOrDefault(material.Id) ?? InventoryItem.From(projectId, material.Id, 0);
            result.Add(InventoryItemView.Create(entity, material));
        }

        return result.ToImmutable();
    }

    public void RemoveInventoryItems(CultivateProject project)
    {
        cultivationRepository.RemoveInventoryItemsByProjectId(project.InnerId);
    }

    public void RefreshInventoryFromYae(CultivateProject project, ImmutableArray<(uint ItemId, uint Count)> storeItems)
    {
        Guid projectId = project.InnerId;
        IEnumerable<InventoryItem> items = storeItems
            .Where(s => s.Count > 0)
            .GroupBy(s => s.ItemId)
            .Select(g => InventoryItem.From(projectId, g.Key, (uint)g.Sum(s => (long)s.Count)));
        cultivationRepository.SaveInventoryItems(projectId, items);
    }

    public void AddCultivateEntryFromAvatar(CultivateProject project, Avatar avatar, CultivateLevelInput levelInput)
    {
        CultivateEntry entry = CultivateEntry.From(project.InnerId, CultivateType.AvatarAndSkill, (uint)avatar.Id);
        cultivationRepository.AddCultivateEntry(entry);

        CultivateEntryLevelInformation levelInfo = new()
        {
            EntryId = entry.InnerId,
            AvatarLevelFrom = levelInput.AvatarLevelFrom,
            AvatarLevelTo = levelInput.AvatarLevelTo,
            SkillALevelFrom = levelInput.SkillALevelFrom,
            SkillALevelTo = levelInput.SkillALevelTo,
            SkillELevelFrom = levelInput.SkillELevelFrom,
            SkillELevelTo = levelInput.SkillELevelTo,
            SkillQLevelFrom = levelInput.SkillQLevelFrom,
            SkillQLevelTo = levelInput.SkillQLevelTo,
        };
        cultivationRepository.AddLevelInformation(levelInfo);

        Dictionary<uint, uint> calculatedMaterials = OfflineCalculator.CalculateAvatarMaterials(avatar, levelInput);

        List<CultivateItem> items = [];
        foreach ((uint materialId, uint count) in calculatedMaterials)
        {
            if (count > 0)
            {
                items.Add(CultivateItem.From(entry.InnerId, materialId, count));
            }
        }

        if (items.Count > 0)
        {
            cultivationRepository.AddCultivateItemRange(items);
        }
    }

    public void AddCultivateEntryFromWeapon(CultivateProject project, Weapon weapon, uint levelFrom, uint levelTo, bool ascended)
    {
        CultivateEntry entry = CultivateEntry.From(project.InnerId, CultivateType.Weapon, (uint)weapon.Id);
        cultivationRepository.AddCultivateEntry(entry);

        CultivateEntryLevelInformation levelInfo = new()
        {
            EntryId = entry.InnerId,
            WeaponLevelFrom = levelFrom,
            WeaponLevelTo = levelTo,
        };
        cultivationRepository.AddLevelInformation(levelInfo);

        Dictionary<uint, uint> calculatedMaterials = OfflineCalculator.CalculateWeaponMaterials(weapon, levelFrom, levelTo, ascended);

        List<CultivateItem> items = [];
        foreach ((uint materialId, uint count) in calculatedMaterials)
        {
            if (count > 0)
            {
                items.Add(CultivateItem.From(entry.InnerId, materialId, count));
            }
        }

        if (items.Count > 0)
        {
            cultivationRepository.AddCultivateItemRange(items);
        }
    }

    public void BatchAddAllAvatarsAndWeapons(CultivateProject project, IEnumerable<Avatar> avatars, IEnumerable<Weapon> weapons, CultivateLevelInput levelInput)
    {
        Guid projectId = project.InnerId;

        List<CultivateEntry> allEntries = [];
        List<CultivateEntryLevelInformation> allLevelInfos = [];
        List<CultivateItem> allItems = [];

        // 收集所有角色数据
        foreach (Avatar avatar in avatars)
        {
            CultivateEntry entry = CultivateEntry.From(projectId, CultivateType.AvatarAndSkill, (uint)avatar.Id);
            entry.InnerId = Guid.NewGuid();
            allEntries.Add(entry);

            allLevelInfos.Add(new CultivateEntryLevelInformation
            {
                EntryId = entry.InnerId,
                AvatarLevelFrom = levelInput.AvatarLevelFrom,
                AvatarLevelTo = levelInput.AvatarLevelTo,
                SkillALevelFrom = levelInput.SkillALevelFrom,
                SkillALevelTo = levelInput.SkillALevelTo,
                SkillELevelFrom = levelInput.SkillELevelFrom,
                SkillELevelTo = levelInput.SkillELevelTo,
                SkillQLevelFrom = levelInput.SkillQLevelFrom,
                SkillQLevelTo = levelInput.SkillQLevelTo,
            });

            Dictionary<uint, uint> materials = OfflineCalculator.CalculateAvatarMaterials(avatar, levelInput);
            foreach ((uint materialId, uint count) in materials)
            {
                if (count > 0)
                {
                    allItems.Add(CultivateItem.From(entry.InnerId, materialId, count));
                }
            }
        }

        // 收集所有武器数据
        foreach (Weapon weapon in weapons)
        {
            CultivateEntry entry = CultivateEntry.From(projectId, CultivateType.Weapon, (uint)weapon.Id);
            entry.InnerId = Guid.NewGuid();
            allEntries.Add(entry);

            allLevelInfos.Add(new CultivateEntryLevelInformation
            {
                EntryId = entry.InnerId,
                WeaponLevelFrom = levelInput.WeaponLevelFrom,
                WeaponLevelTo = levelInput.WeaponLevelTo,
            });

            Dictionary<uint, uint> materials = OfflineCalculator.CalculateWeaponMaterials(weapon, levelInput.WeaponLevelFrom, levelInput.WeaponLevelTo, levelInput.WeaponAscended);
            foreach ((uint materialId, uint count) in materials)
            {
                if (count > 0)
                {
                    allItems.Add(CultivateItem.From(entry.InnerId, materialId, count));
                }
            }
        }

        // 一次性写入数据库（只触发 3 次 SaveChanges）
        cultivationRepository.AddCultivateEntryRange(allEntries);
        cultivationRepository.AddLevelInformationRange(allLevelInfos);
        cultivationRepository.AddCultivateItemRange(allItems);
    }
}
