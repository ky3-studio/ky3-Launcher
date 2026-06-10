//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.ViewModel.Achievement;
using System.Collections.Immutable;
using EntityAchievement = kyxsan.Model.Entity.Achievement;

namespace kyxsan.Service.Achievement;

[Service(ServiceLifetime.Transient, typeof(IAchievementStatisticsService))]
internal sealed partial class AchievementStatisticsService : IAchievementStatisticsService
{
    private const int AchievementCardTakeCount = 2;

    private readonly IAchievementRepository achievementRepository;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial AchievementStatisticsService(IServiceProvider serviceProvider);

    public async ValueTask<ImmutableArray<AchievementStatistics>> GetAchievementStatisticsAsync(AchievementServiceMetadataContext context, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        return SynchronizedGetAchievementStatistics(context);
    }

    private ImmutableArray<AchievementStatistics> SynchronizedGetAchievementStatistics(AchievementServiceMetadataContext context)
    {
        ImmutableArray<AchievementStatistics>.Builder results = ImmutableArray.CreateBuilder<AchievementStatistics>();
        foreach (AchievementArchive archive in achievementRepository.GetAchievementArchiveImmutableArray())
        {
            int finishedCount = achievementRepository.GetFinishedAchievementCountByArchiveId(archive.InnerId);
            int totalCount = context.IdAchievementMap.Count;
            ImmutableArray<EntityAchievement> achievements = achievementRepository.GetLatestFinishedAchievementImmutableArrayByArchiveId(archive.InnerId, AchievementCardTakeCount);

            results.Add(new()
            {
                DisplayName = archive.Name,
                FinishDescription = AchievementStatistics.Format(finishedCount, totalCount, out _),
                Achievements = achievements.SelectAsArray(static (entity, context) => AchievementView.Create(entity, context.IdAchievementMap[entity.Id]), context),
            });
        }

        return results.ToImmutable();
    }
}