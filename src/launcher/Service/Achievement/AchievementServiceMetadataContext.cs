//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;
using MetadataAchievement = kyxsan.Model.Metadata.Achievement.Achievement;

namespace kyxsan.Service.Achievement;

internal sealed class AchievementServiceMetadataContext : IMetadataContext,
    IMetadataArrayAchievementSource,
    IMetadataDictionaryIdAchievementSource
{
    public ImmutableArray<MetadataAchievement> Achievements { get; set; } = default!;

    public ImmutableDictionary<AchievementId, MetadataAchievement> IdAchievementMap { get; set; } = default!;
}