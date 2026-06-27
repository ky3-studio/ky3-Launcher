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

[Service(ServiceLifetime.Scoped, typeof(ILauncherSpiralAbyssService))]
internal sealed partial class LauncherSpiralAbyssService : ObjectCacheService, ILauncherSpiralAbyssService
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial LauncherSpiralAbyssService(IServiceProvider serviceProvider);

    public override string TypeName { get; } = nameof(LauncherSpiralAbyssService);

    public async ValueTask<Overview> GetOverviewAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(Overview), last, homaClient.GetOverviewAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarAppearanceRank), last, homaClient.GetAvatarAttendanceRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarUsageRank), last, homaClient.GetAvatarUtilizationRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarConstellationInfo), last, homaClient.GetAvatarHoldingRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarCollocation), last, homaClient.GetAvatarCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(WeaponCollocation), last, homaClient.GetWeaponCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<TeamAppearance>> GetTeamAppearancesAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            LauncherSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<LauncherSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(TeamAppearance), last, homaClient.GetTeamCombinationsAsync).ConfigureAwait(false);
        }
    }
}