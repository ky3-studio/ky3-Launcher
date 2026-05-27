//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Service.Achievement;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.UI.Xaml.View.Page;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Achievement;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class AchievementViewModelSlim : Abstraction.ViewModelSlim<AchievementPage>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial AchievementViewModelSlim(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial ImmutableArray<AchievementStatistics> StatisticsList { get; set; } = [];

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();

            if (!await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                return;
            }

            AchievementServiceMetadataContext context = await metadataService
                .GetContextAsync<AchievementServiceMetadataContext>()
                .ConfigureAwait(false);
            ImmutableArray<AchievementStatistics> array = await scope.ServiceProvider
                .GetRequiredService<IAchievementStatisticsService>()
                .GetAchievementStatisticsAsync(context)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            StatisticsList = array;
            IsInitialized = true;
        }
    }
}