//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Primitive;
using Launcher.ViewModel.Complex;
using Launcher.Web.Launcher.SpiralAbyss;
using System.Collections.Immutable;

namespace Launcher.Service.Launcher;

internal interface ILauncherSpiralAbyssStatisticsCache
{
    ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set; }

    ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set; }

    ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set; }

    ImmutableArray<TeamAppearanceView> TeamAppearances { get; set; }

    Overview? Overview { get; set; }

    ImmutableDictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    ImmutableDictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    ValueTask InitializeForSpiralAbyssViewAsync(LauncherSpiralAbyssStatisticsMetadataContext context);

    ValueTask InitializeForWikiAvatarViewAsync(LauncherSpiralAbyssStatisticsMetadataContext context);

    ValueTask InitializeForWikiWeaponViewAsync(LauncherSpiralAbyssStatisticsMetadataContext context);
}
