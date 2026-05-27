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
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Complex;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class kyxsanRoleCombatDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IkyxsanRoleCombatStatisticsCache kyxsanCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial kyxsanRoleCombatDatabaseViewModel(IServiceProvider serviceProvider);

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

        kyxsanRoleCombatStatisticsMetadataContext context = await metadataService.GetContextAsync<kyxsanRoleCombatStatisticsMetadataContext>().ConfigureAwait(false);
        await kyxsanCache.InitializeForRoleCombatViewAsync(context).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        RecordTotal = kyxsanCache.RecordTotal;
        AvatarAppearances = kyxsanCache.AvatarAppearances;
    }
}