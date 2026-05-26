//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database;
using kyxsan.Model.Entity;

namespace kyxsan.Service.GachaLog;

internal static class GachaArchiveOperation
{
    public static GachaArchive GetOrAdd(IGachaLogRepository repository, string uid, IAdvancedDbCollectionView<GachaArchive> archives)
    {
        GachaArchive? archive = archives.Source.SingleOrDefault(a => a.Uid == uid);

        if (archive is not null)
        {
            return archive;
        }

        GachaArchive created = GachaArchive.Create(uid);
        repository.AddGachaArchive(created);
        CollectionViewAddGachaArchive(archives, created, repository.ServiceProvider.GetRequiredService<ITaskContext>());
        return created;
    }

    private static void CollectionViewAddGachaArchive(IAdvancedDbCollectionView<GachaArchive> archives, GachaArchive archive, ITaskContext taskContext)
    {
        taskContext.InvokeOnMainThread(() => archives.Add(archive));
    }
}
