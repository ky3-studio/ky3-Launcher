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
using Launcher.ViewModel.Complex;
using Launcher.ViewModel.SpiralAbyss;
using Launcher.ViewModel.Wiki;
using Launcher.Web.Launcher.SpiralAbyss;
using System.Collections.Immutable;
using AvatarView = Launcher.ViewModel.Complex.AvatarView;

namespace Launcher.Service.Launcher;

[Service(ServiceLifetime.Singleton, typeof(ILauncherSpiralAbyssStatisticsCache))]
internal sealed partial class LauncherSpiralAbyssStatisticsCache : StatisticsCache, ILauncherSpiralAbyssStatisticsCache
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial LauncherSpiralAbyssStatisticsCache(IServiceProvider serviceProvider);

    public ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set; }

    public ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set; }

    public ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set; }

    public ImmutableArray<TeamAppearanceView> TeamAppearances { get; set; }

    public Overview? Overview { get; set; }

    public ImmutableDictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    public ImmutableDictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    public ValueTask InitializeForSpiralAbyssViewAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<SpiralAbyssViewModel, LauncherSpiralAbyssStatisticsMetadataContext>(context, context =>
        {
            ReadOnlySpan<Task> tasks =
            [
                AvatarAppearanceRankAsync(context),
                AvatarUsageRanksAsync(context),
                AvatarConstellationInfosAsync(context),
                TeamAppearancesAsync(context),
                OverviewAsync(),
            ];

            return Task.WhenAll(tasks);
        });
    }

    public ValueTask InitializeForWikiAvatarViewAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<WikiAvatarViewModel, LauncherSpiralAbyssStatisticsMetadataContext>(context, AvatarCollocationsAsync);
    }

    public ValueTask InitializeForWikiWeaponViewAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<WikiWeaponViewModel, LauncherSpiralAbyssStatisticsMetadataContext>(context, WeaponCollocationsAsync);
    }


    [SuppressMessage("", "SH003")]
    private async Task AvatarCollocationsAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            raw = await LauncherService.GetAvatarCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await LauncherService.GetAvatarCollocationsAsync(true).ConfigureAwait(false);
        }

        AvatarCollocations = CurrentJoinLast(raw, rawLast, data => data.AvatarId, (raw, rawLast) => new AvatarCollocationView
        {
            AvatarId = raw.AvatarId,
            Avatars = [.. CurrentJoinLast(raw.Avatars, rawLast?.Avatars, data => data.Item, (avatar, avatarLast) => new AvatarView(context.GetAvatar(avatar.Item), avatar.Rate, avatarLast?.Rate))],
            Weapons = [.. CurrentJoinLast(raw.Weapons, rawLast?.Weapons, data => data.Item, (weapon, weaponLast) => new WeaponView(context.GetWeapon(weapon.Item), weapon.Rate, weaponLast?.Rate))],
            ReliquarySets = [.. CurrentJoinLast(raw.Reliquaries, rawLast?.Reliquaries, data => data.Item, (relic, relicLast) => new ReliquarySetView(context.ExtendedIdReliquarySetMap, relic, relicLast))],
        }).ToImmutableDictionary(a => a.AvatarId);
    }

    [SuppressMessage("", "SH003")]
    private async Task WeaponCollocationsAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<WeaponCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            raw = await LauncherService.GetWeaponCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await LauncherService.GetWeaponCollocationsAsync(true).ConfigureAwait(false);
        }

        WeaponCollocations = CurrentJoinLast(raw, rawLast, data => data.WeaponId, (raw, rawLast) => new WeaponCollocationView
        {
            WeaponId = raw.WeaponId,
            Avatars = [.. CurrentJoinLast(raw.Avatars, rawLast?.Avatars, data => data.Item, (avatar, avatarLast) => new AvatarView(context.GetAvatar(avatar.Item), avatar.Rate, avatarLast?.Rate))],
        }).ToImmutableDictionary(w => w.WeaponId);
    }


    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearanceRankAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarAppearanceRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            raw = await LauncherService.GetAvatarAppearanceRanksAsync(false).ConfigureAwait(false);
            rawLast = await LauncherService.GetAvatarAppearanceRanksAsync(true).ConfigureAwait(false);
        }

        AvatarAppearanceRanks =
        [
            .. CurrentJoinLast(raw.OrderByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
            {
                Floor = SH.FormatModelBindingLauncherComplexRankFloor(raw.Floor),
                Avatars = [..CurrentJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(context.GetAvatar(rank.Item), rank.Rate, rankLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarUsageRanksAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarUsageRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            raw = await LauncherService.GetAvatarUsageRanksAsync(false).ConfigureAwait(false);
            rawLast = await LauncherService.GetAvatarUsageRanksAsync(true).ConfigureAwait(false);
        }

        AvatarUsageRanks =
        [
            .. CurrentJoinLast(raw.OrderByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
            {
                Floor = SH.FormatModelBindingLauncherComplexRankFloor(raw.Floor),
                Avatars = [.. CurrentJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(context.GetAvatar(rank.Item), rank.Rate, rankLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarConstellationInfosAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarConstellationInfo> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            raw = await LauncherService.GetAvatarConstellationInfosAsync(false).ConfigureAwait(false);
            rawLast = await LauncherService.GetAvatarConstellationInfosAsync(true).ConfigureAwait(false);
        }

        AvatarConstellationInfos =
        [
            .. CurrentJoinLast(raw.OrderBy(i => i.HoldingRate), rawLast, data => data.AvatarId, (raw, rawLast) => new AvatarConstellationInfoView(context.GetAvatar(raw.AvatarId), raw.HoldingRate, rawLast?.HoldingRate)
            {
                Rates = [.. CurrentJoinLast(raw.Constellations, rawLast?.Constellations, data => data.Item, (rate, rateLast) => new RateAndDelta(rate.Rate, rateLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task TeamAppearancesAsync(LauncherSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<TeamAppearance> teamAppearancesRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            teamAppearancesRaw = await LauncherService.GetTeamAppearancesAsync().ConfigureAwait(false);
        }

        TeamAppearances = [.. teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new TeamAppearanceView(team, context.IdAvatarMap))];
    }

    [SuppressMessage("", "SH003")]
    private async Task OverviewAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherSpiralAbyssService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherSpiralAbyssService>();
            Overview = await LauncherService.GetOverviewAsync().ConfigureAwait(false);
        }
    }
}