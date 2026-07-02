//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Service.Launcher;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Web.Launcher.SpiralAbyss;
using System.Collections.Immutable;

namespace Launcher.ViewModel.Complex;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class LauncherSpiralAbyssDatabaseViewModel : Abstraction.ViewModel
{
    private readonly ILauncherSpiralAbyssStatisticsCache LauncherCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial LauncherSpiralAbyssDatabaseViewModel(IServiceProvider serviceProvider);

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

        LauncherSpiralAbyssStatisticsMetadataContext context = await metadataService.GetContextAsync<LauncherSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
        await LauncherCache.InitializeForSpiralAbyssViewAsync(context).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        AvatarAppearanceRanks = LauncherCache.AvatarAppearanceRanks;
        SelectedAvatarAppearanceRank = AvatarAppearanceRanks.FirstOrDefault();

        AvatarUsageRanks = LauncherCache.AvatarUsageRanks;
        SelectedAvatarUsageRank = AvatarUsageRanks.FirstOrDefault();

        TeamAppearances = LauncherCache.TeamAppearances;
        SelectedTeamAppearance = TeamAppearances.FirstOrDefault();

        AvatarConstellationInfos = LauncherCache.AvatarConstellationInfos;
        Overview = LauncherCache.Overview;
    }
}
