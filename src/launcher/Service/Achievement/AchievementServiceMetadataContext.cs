//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Primitive;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableArray;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;
using MetadataAchievement = Launcher.Model.Metadata.Achievement.Achievement;

namespace Launcher.Service.Achievement;

internal sealed class AchievementServiceMetadataContext : IMetadataContext,
    IMetadataArrayAchievementSource,
    IMetadataDictionaryIdAchievementSource
{
    public ImmutableArray<MetadataAchievement> Achievements { get; set; } = default!;

    public ImmutableDictionary<AchievementId, MetadataAchievement> IdAchievementMap { get; set; } = default!;
}
