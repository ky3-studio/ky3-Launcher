//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Core.Database;
using kyxsan.Core.Diagnostics;
using kyxsan.Core.ExceptionService;
using kyxsan.Model.Entity;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.Service.GachaLog.Factory;
using kyxsan.Service.GachaLog.QueryProvider;
using kyxsan.Service.GachaLog.UIGF;
using Avatar = kyxsan.Model.Metadata.Avatar.Avatar;
using Weapon = kyxsan.Model.Metadata.Weapon.Weapon;
using kyxsan.ViewModel.GachaLog;
using kyxsan.Web.Hoyolab.Hk4e.Event.GachaInfo;
using kyxsan.Web.Response;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;

namespace kyxsan.Service.GachaLog;

[Service(ServiceLifetime.Scoped, typeof(IGachaLogService))]
internal sealed partial class GachaLogService : IGachaLogService
{
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<GachaLogService> logger;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock archivesLock = new();
    private IAdvancedDbCollectionView<GachaArchive>? archives;

    [GeneratedConstructor]
    public partial GachaLogService(IServiceProvider serviceProvider);

    public async ValueTask<IAdvancedDbCollectionView<GachaArchive>> GetArchiveCollectionAsync()
    {
        using (await archivesLock.LockAsync().ConfigureAwait(false))
        {
            return archives ??= gachaLogRepository.GetGachaArchiveCollection().ToAdvancedDbCollectionView(serviceProvider);
        }
    }

    public async ValueTask<GachaStatistics> GetStatisticsAsync(GachaLogServiceMetadataContext context, GachaArchive archive)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            ImmutableArray<GachaItem> items = gachaLogRepository.GetGachaItemImmutableArrayByArchiveId(archive.InnerId);
            return await gachaStatisticsFactory.CreateAsync(context, items).ConfigureAwait(false);
        }
    }

    public async ValueTask<bool> RefreshGachaLogAsync(GachaLogServiceMetadataContext context, GachaLogQuery query, RefreshStrategyKind strategy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        bool isLazy = strategy switch
        {
            RefreshStrategyKind.AggressiveMerge => false,
            RefreshStrategyKind.LazyMerge => true,
            _ => throw kyxsanException.NotSupported(),
        };

        (bool authkeyValid, GachaArchive? target) = await FetchGachaLogsAsync(context, query, isLazy, progress, token).ConfigureAwait(false);

        if (target is not null)
        {
            IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            localArchives.MoveCurrentTo(target);
        }

        return authkeyValid;
    }

    public async ValueTask RemoveArchiveAsync(GachaArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        gachaLogRepository.DeleteGachaArchiveById(archive.InnerId);

        IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        localArchives.Remove(archive);
        localArchives.MoveCurrentToFirst();
    }

    public async ValueTask<GachaArchive> EnsureArchiveInCollectionAsync(Guid archiveId, CancellationToken token = default)
    {
        IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
        if (localArchives.Source.SingleOrDefault(a => a.InnerId == archiveId) is { } archive)
        {
            return archive;
        }

        GachaArchive? newArchive = gachaLogRepository.GetGachaArchiveById(archiveId);
        ArgumentNullException.ThrowIfNull(newArchive);

        await taskContext.SwitchToMainThreadAsync();
        localArchives.Add(newArchive);
        return newArchive;
    }

    public async ValueTask<(int Count, Guid ArchiveId)> ImportFromUIGFAsync(GachaLogServiceMetadataContext? metadata, Stream stream)
    {
        await taskContext.SwitchToBackgroundAsync();

        UIGFRoot? uigf = await JsonSerializer.DeserializeAsync<UIGFRoot>(stream, Core.Text.Json.JsonOptions.Default).ConfigureAwait(false);
        if (uigf is null)
        {
            return (0, default);
        }

        int totalImported = 0;
        Guid lastArchiveId = default;

        if (uigf.Hk4e is { Count: > 0 })
        {
            foreach (UIGFEntry entry in uigf.Hk4e)
            {
                string uid = entry.Uid.ToString(CultureInfo.InvariantCulture);
                (int count, Guid archiveId) = ImportItemsForUid(uid, entry.List, entry.Timezone, metadata);
                totalImported += count;
                if (archiveId != default)
                {
                    lastArchiveId = archiveId;
                }
            }
        }
        else if (uigf.List is { Count: > 0 } && !string.IsNullOrEmpty(uigf.Info.Uid))
        {
            int timezone = uigf.Info.RegionTimeZone ?? 8;
            (int count, Guid archiveId) = ImportItemsForUid(uigf.Info.Uid, uigf.List, timezone, metadata);
            totalImported += count;
            lastArchiveId = archiveId;
        }

        return (totalImported, lastArchiveId);
    }

    private (int Count, Guid ArchiveId) ImportItemsForUid(string uid, List<UIGFItem> items, int timezone, GachaLogServiceMetadataContext? metadata)
    {
        if (string.IsNullOrEmpty(uid) || items.Count == 0)
        {
            return (0, default);
        }

        GachaArchive? archive = gachaLogRepository.GetGachaArchiveByUid(uid);
        if (archive is null)
        {
            archive = GachaArchive.Create(uid);
            gachaLogRepository.AddGachaArchive(archive);
        }

        HashSet<long> existingIds = gachaLogRepository
            .GetGachaItemListByArchiveId(archive.InnerId)
            .Select(i => i.Id)
            .ToHashSet();

        List<GachaItem> newItems = [];
        foreach (UIGFItem uigfItem in items)
        {
            if (existingIds.Contains(uigfItem.Id))
            {
                continue;
            }

            GachaType queryType = uigfItem.UIGFGachaType != 0 ? uigfItem.UIGFGachaType : uigfItem.GachaType;
            uint resolvedItemId = ResolveItemId(uigfItem, metadata);

            newItems.Add(new GachaItem
            {
                ArchiveId = archive.InnerId,
                GachaType = uigfItem.GachaType,
                QueryType = queryType,
                ItemId = resolvedItemId,
                Time = new DateTimeOffset(uigfItem.Time, TimeSpan.FromHours(timezone)),
                Id = uigfItem.Id,
            });
        }

        if (newItems.Count > 0)
        {
            gachaLogRepository.AddGachaItemRange(newItems);
        }

        return (newItems.Count, archive.InnerId);
    }

    private static uint ResolveItemId(UIGFItem uigfItem, GachaLogServiceMetadataContext? metadata)
    {
        if (uint.TryParse(uigfItem.ItemId, out uint parsedId) && parsedId > 0)
        {
            return parsedId;
        }

        if (!string.IsNullOrEmpty(uigfItem.Name) && metadata is not null)
        {
            if (metadata.NameAvatarMap.TryGetValue(uigfItem.Name, out Avatar? avatar))
            {
                return avatar.Id;
            }

            if (metadata.NameWeaponMap.TryGetValue(uigfItem.Name, out Weapon? weapon))
            {
                return weapon.Id;
            }
        }

        return 0;
    }

    public async ValueTask ExportToUIGFAsync(GachaArchive archive, Stream stream, bool useLegacyFormat = false, GachaLogServiceMetadataContext? metadata = null)
    {
        await taskContext.SwitchToBackgroundAsync();

        List<GachaItem> items = gachaLogRepository.GetGachaItemListByArchiveId(archive.InnerId);

        TimeSpan tz = TimeSpan.FromHours(8);
        List<UIGFItem> uigfItems = items.Select(i =>
        {
            UIGFItem uigfItem = new()
            {
                UIGFGachaType = i.QueryType,
                GachaType = i.GachaType,
                ItemId = i.ItemId.ToString(CultureInfo.InvariantCulture),
                Time = i.Time.ToOffset(tz).DateTime,
                Id = i.Id,
            };

            if (metadata is not null && i.ItemId > 0)
            {
                INameQualityAccess nq = metadata.GetNameQualityByItemId(i.ItemId);
                uigfItem.Name = nq.Name;
                uigfItem.RankType = ((int)nq.Quality).ToString(CultureInfo.InvariantCulture);
                uigfItem.ItemType = i.ItemId.StringLength switch
                {
                    8U => SH.ModelInterchangeUIGFItemTypeAvatar,
                    5U => SH.ModelInterchangeUIGFItemTypeWeapon,
                    _ => null,
                };
                uigfItem.Count = "1";
            }

            return uigfItem;
        }).ToList();

        UIGFRoot uigf;

        if (useLegacyFormat)
        {
            uigf = new()
            {
                Info = new UIGFInfo
                {
                    ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    ExportApp = "kyxsan",
                    ExportAppVersion = kyxsanRuntime.Version.ToString(),
                    UIGFVersion = "v2.3",
                    Uid = archive.Uid,
                    Lang = "zh-cn",
                    RegionTimeZone = 8,
                },
                List = uigfItems,
            };
        }
        else
        {
            uigf = new()
            {
                Info = new UIGFInfo
                {
                    ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    ExportApp = "kyxsan",
                    ExportAppVersion = kyxsanRuntime.Version.ToString(),
                    Version = "v4.0",
                },
                Hk4e =
                [
                    new UIGFEntry
                    {
                        Uid = uint.Parse(archive.Uid, CultureInfo.InvariantCulture),
                        List = uigfItems,
                    },
                ],
            };
        }

        await JsonSerializer.SerializeAsync(stream, uigf, Core.Text.Json.JsonOptions.Default).ConfigureAwait(false);
    }

    private async ValueTask<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogServiceMetadataContext context, GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
        GachaLogFetchContext fetchContext = new(gachaLogRepository, context, isLazy);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            GachaInfoClient gachaInfoClient = scope.ServiceProvider.GetRequiredService<GachaInfoClient>();

            foreach (GachaType configType in GachaLog.QueryTypes)
            {
                fetchContext.ResetType(configType, query);

                do
                {
                    Response<GachaLogPage> response = await gachaInfoClient
                        .GetGachaLogPageAsync(fetchContext.TypedQueryOptions, token)
                        .ConfigureAwait(false);

                    if (!ResponseValidator.TryValidateWithoutUINotification(response, out GachaLogPage? page))
                    {
                        fetchContext.Report(progress, isAuthKeyTimeout: true);
                        break;
                    }

                    fetchContext.ResetCurrentPage();
                    ImmutableArray<GachaLogItem> items = page.List;

                    foreach (GachaLogItem item in items)
                    {
                        fetchContext.EnsureArchiveAndEndId(item, localArchives, gachaLogRepository);

                        if (fetchContext.ShouldAddItem(item))
                        {
                            fetchContext.AddItem(item);
                        }
                        else
                        {
                            fetchContext.CompleteCurrentTypeAdding();
                            break;
                        }
                    }

                    fetchContext.Report(progress);
                    await Task.Delay(System.Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);

                    if (fetchContext.HasReachCurrentTypeEnd(items))
                    {
                        break;
                    }
                }
                while (true);

                if (fetchContext.Status.AuthKeyTimeout)
                {
                    break;
                }

                token.ThrowIfCancellationRequested();
                fetchContext.SaveItems();

                await Task.Delay(System.Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);
            }
        }

        return new(!fetchContext.Status.AuthKeyTimeout, fetchContext.TargetArchive);
    }
}
