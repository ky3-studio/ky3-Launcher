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
using kyxsan.ViewModel.Complex;
using kyxsan.ViewModel.SpiralAbyss;
using kyxsan.ViewModel.Wiki;
using kyxsan.Web.kyxsan.SpiralAbyss;
using System.Collections.Immutable;
using AvatarView = kyxsan.ViewModel.Complex.AvatarView;

namespace kyxsan.Service.kyxsan;

[Service(ServiceLifetime.Singleton, typeof(IkyxsanSpiralAbyssStatisticsCache))]
internal sealed partial class kyxsanSpiralAbyssStatisticsCache : StatisticsCache, IkyxsanSpiralAbyssStatisticsCache
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial kyxsanSpiralAbyssStatisticsCache(IServiceProvider serviceProvider);

    public ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set; }

    public ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set; }

    public ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set; }

    public ImmutableArray<TeamAppearanceView> TeamAppearances { get; set; }

    public Overview? Overview { get; set; }

    public ImmutableDictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    public ImmutableDictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    public ValueTask InitializeForSpiralAbyssViewAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<SpiralAbyssViewModel, kyxsanSpiralAbyssStatisticsMetadataContext>(context, context =>
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

    public ValueTask InitializeForWikiAvatarViewAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<WikiAvatarViewModel, kyxsanSpiralAbyssStatisticsMetadataContext>(context, AvatarCollocationsAsync);
    }


    [SuppressMessage("", "SH003")]
    private async Task AvatarCollocationsAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanSpiralAbyssService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanSpiralAbyssService>();
            raw = await kyxsanService.GetAvatarCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await kyxsanService.GetAvatarCollocationsAsync(true).ConfigureAwait(false);
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
    private async Task AvatarAppearanceRankAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarAppearanceRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanSpiralAbyssService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanSpiralAbyssService>();
            raw = await kyxsanService.GetAvatarAppearanceRanksAsync(false).ConfigureAwait(false);
            rawLast = await kyxsanService.GetAvatarAppearanceRanksAsync(true).ConfigureAwait(false);
        }

        AvatarAppearanceRanks =
        [
            .. CurrentJoinLast(raw.OrderByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
            {
                Floor = SH.FormatModelBindingkyxsanComplexRankFloor(raw.Floor),
                Avatars = [..CurrentJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(context.GetAvatar(rank.Item), rank.Rate, rankLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarUsageRanksAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarUsageRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanSpiralAbyssService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanSpiralAbyssService>();
            raw = await kyxsanService.GetAvatarUsageRanksAsync(false).ConfigureAwait(false);
            rawLast = await kyxsanService.GetAvatarUsageRanksAsync(true).ConfigureAwait(false);
        }

        AvatarUsageRanks =
        [
            .. CurrentJoinLast(raw.OrderByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
            {
                Floor = SH.FormatModelBindingkyxsanComplexRankFloor(raw.Floor),
                Avatars = [.. CurrentJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(context.GetAvatar(rank.Item), rank.Rate, rankLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarConstellationInfosAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarConstellationInfo> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanSpiralAbyssService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanSpiralAbyssService>();
            raw = await kyxsanService.GetAvatarConstellationInfosAsync(false).ConfigureAwait(false);
            rawLast = await kyxsanService.GetAvatarConstellationInfosAsync(true).ConfigureAwait(false);
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
    private async Task TeamAppearancesAsync(kyxsanSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<TeamAppearance> teamAppearancesRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanSpiralAbyssService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanSpiralAbyssService>();
            teamAppearancesRaw = await kyxsanService.GetTeamAppearancesAsync().ConfigureAwait(false);
        }

        TeamAppearances = [.. teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new TeamAppearanceView(team, context.IdAvatarMap))];
    }

    [SuppressMessage("", "SH003")]
    private async Task OverviewAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanSpiralAbyssService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanSpiralAbyssService>();
            Overview = await kyxsanService.GetOverviewAsync().ConfigureAwait(false);
        }
    }
}