//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Web.kyxsan.SpiralAbyss;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Complex;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class kyxsanSpiralAbyssDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IkyxsanSpiralAbyssStatisticsCache kyxsanCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial kyxsanSpiralAbyssDatabaseViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set; }

    [ObservableProperty]
    public partial AvatarRankView? SelectedAvatarUsageRank { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set; }

    [ObservableProperty]
    public partial AvatarRankView? SelectedAvatarAppearanceRank { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<TeamAppearanceView> TeamAppearances { get; set; }

    [ObservableProperty]
    public partial TeamAppearanceView? SelectedTeamAppearance { get; set; }

    [ObservableProperty]
    public partial Overview? Overview { get; set; }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        kyxsanSpiralAbyssStatisticsMetadataContext context = await metadataService.GetContextAsync<kyxsanSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
        await kyxsanCache.InitializeForSpiralAbyssViewAsync(context).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        AvatarAppearanceRanks = kyxsanCache.AvatarAppearanceRanks;
        SelectedAvatarAppearanceRank = AvatarAppearanceRanks.FirstOrDefault();

        AvatarUsageRanks = kyxsanCache.AvatarUsageRanks;
        SelectedAvatarUsageRank = AvatarUsageRanks.FirstOrDefault();

        TeamAppearances = kyxsanCache.TeamAppearances;
        SelectedTeamAppearance = TeamAppearances.FirstOrDefault();

        AvatarConstellationInfos = kyxsanCache.AvatarConstellationInfos;
        Overview = kyxsanCache.Overview;
    }
}