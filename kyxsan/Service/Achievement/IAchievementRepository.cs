//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using kyxsan.Service.Abstraction;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using EntityAchievement = kyxsan.Model.Entity.Achievement;
using EntityArchive = kyxsan.Model.Entity.AchievementArchive;

namespace kyxsan.Service.Achievement;

internal interface IAchievementRepository : IRepository<EntityArchive>, IRepository<EntityAchievement>
{
    void AddAchievementArchive(EntityArchive archive);

    ObservableCollection<EntityArchive> GetAchievementArchiveCollection();

    ImmutableArray<EntityArchive> GetAchievementArchiveImmutableArray();

    EntityArchive? GetAchievementArchiveById(Guid archiveId);

    EntityArchive? GetAchievementArchiveByName(string name);

    void RemoveAchievementArchive(EntityArchive archive);

    ImmutableArray<EntityAchievement> GetAchievementImmutableArrayByArchiveId(Guid archiveId);

    FrozenDictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);

    int GetFinishedAchievementCountByArchiveId(Guid archiveId);

    ImmutableArray<EntityAchievement> GetLatestFinishedAchievementImmutableArrayByArchiveId(Guid archiveId, int take);

    void OverwriteAchievement(EntityAchievement achievement);
}