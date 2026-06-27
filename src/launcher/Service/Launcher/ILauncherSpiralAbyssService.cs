//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Web.Launcher.SpiralAbyss;

namespace Launcher.Service.Launcher;

internal interface ILauncherSpiralAbyssService
{
    ValueTask<IReadOnlyList<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false);

    ValueTask<IReadOnlyList<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false);

    ValueTask<IReadOnlyList<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false);

    ValueTask<IReadOnlyList<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false);

    ValueTask<Overview> GetOverviewAsync(bool last = false);

    ValueTask<IReadOnlyList<TeamAppearance>> GetTeamAppearancesAsync(bool last = false);

    ValueTask<IReadOnlyList<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false);
}