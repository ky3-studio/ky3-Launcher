//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Model.InterChange.Achievement;
using Launcher.UI.Xaml.Data;
using Launcher.ViewModel.Achievement;
using System.Collections.Immutable;
using EntityArchive = Launcher.Model.Entity.AchievementArchive;

namespace Launcher.Service.Achievement;

internal interface IAchievementService
{
    ValueTask<IAdvancedDbCollectionView<EntityArchive>> GetArchiveCollectionAsync(CancellationToken token = default);

    ValueTask<UIAF> ExportToUIAFAsync(EntityArchive archive);

    IAdvancedCollectionView<AchievementView> GetAchievementViewCollection(EntityArchive archive, AchievementServiceMetadataContext context);

    ValueTask<ImportResult> ImportFromUIAFAsync(EntityArchive archive, ImmutableArray<UIAFItem> array, ImportStrategyKind strategy);

    ValueTask RemoveArchiveAsync(EntityArchive archive);

    void SaveAchievement(AchievementView achievement);

    ValueTask<ArchiveAddResultKind> AddArchiveAsync(EntityArchive newArchive);
}