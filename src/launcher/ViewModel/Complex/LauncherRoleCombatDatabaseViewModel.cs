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
using System.Collections.Immutable;

namespace Launcher.ViewModel.Complex;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class LauncherRoleCombatDatabaseViewModel : Abstraction.ViewModel
{
    private readonly ILauncherRoleCombatStatisticsCache LauncherCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial LauncherRoleCombatDatabaseViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial int RecordTotal { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<AvatarView> AvatarAppearances { get; set; }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        LauncherRoleCombatStatisticsMetadataContext context = await metadataService.GetContextAsync<LauncherRoleCombatStatisticsMetadataContext>().ConfigureAwait(false);
        await LauncherCache.InitializeForRoleCombatViewAsync(context).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        RecordTotal = LauncherCache.RecordTotal;
        AvatarAppearances = LauncherCache.AvatarAppearances;
    }
}
