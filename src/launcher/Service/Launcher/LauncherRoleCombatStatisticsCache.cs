//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.ViewModel.RoleCombat;
using Launcher.Web.Launcher.RoleCombat;
using System.Collections.Immutable;
using AvatarView = Launcher.ViewModel.Complex.AvatarView;

namespace Launcher.Service.Launcher;

[Service(ServiceLifetime.Singleton, typeof(ILauncherRoleCombatStatisticsCache))]
internal sealed partial class LauncherRoleCombatStatisticsCache : StatisticsCache, ILauncherRoleCombatStatisticsCache
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial LauncherRoleCombatStatisticsCache(IServiceProvider serviceProvider);

    public int RecordTotal { get; private set; }

    public ImmutableArray<AvatarView> AvatarAppearances { get; private set; }

    public ValueTask InitializeForRoleCombatViewAsync(LauncherRoleCombatStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<RoleCombatViewModel, LauncherRoleCombatStatisticsMetadataContext>(context, AvatarAppearancesAsync);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearancesAsync(LauncherRoleCombatStatisticsMetadataContext context)
    {
        RoleCombatStatisticsItem raw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ILauncherRoleCombatService LauncherService = scope.ServiceProvider.GetRequiredService<ILauncherRoleCombatService>();
            raw = await LauncherService.GetRoleCombatStatisticsItemAsync().ConfigureAwait(false);
        }

        RecordTotal = raw.RecordTotal;
        AvatarAppearances = [.. CurrentJoinLast(raw.BackupAvatarRates.EmptyIfDefault().OrderByDescending(ir => ir.Rate), default, data => data.Item, (data, dataLast) => new AvatarView(context.GetAvatar(data.Item), data.Rate, dataLast?.Rate))];
    }
}