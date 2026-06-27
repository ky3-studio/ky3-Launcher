//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Entity;
using Launcher.Model.Intrinsic;
using Launcher.Service.Abstraction;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Launcher.Service.GachaLog;

internal interface IGachaLogRepository : IRepository<GachaArchive>
{
    ObservableCollection<GachaArchive> GetGachaArchiveCollection();

    GachaArchive? GetGachaArchiveByUid(string uid);

    GachaArchive? GetGachaArchiveById(Guid archiveId);

    void AddGachaArchive(GachaArchive archive);

    void DeleteGachaArchiveById(Guid archiveId);

    List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId);

    ImmutableArray<GachaItem> GetGachaItemImmutableArrayByArchiveId(Guid archiveId);

    long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);

    long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);

    void AddGachaItemRange(IEnumerable<GachaItem> items);

    void RemoveGachaItemRangeByArchiveId(Guid archiveId);

    void RemoveGachaItemRangeByArchiveIdAndQueryTypeNewerThanEndId(Guid archiveId, GachaType queryType, long endId);
}
