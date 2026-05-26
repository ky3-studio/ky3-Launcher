//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.ViewModel.RoleCombat;
using kyxsan.Web.kyxsan.RoleCombat;
using System.Collections.Immutable;
using AvatarView = kyxsan.ViewModel.Complex.AvatarView;

namespace kyxsan.Service.kyxsan;

[Service(ServiceLifetime.Singleton, typeof(IkyxsanRoleCombatStatisticsCache))]
internal sealed partial class kyxsanRoleCombatStatisticsCache : StatisticsCache, IkyxsanRoleCombatStatisticsCache
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial kyxsanRoleCombatStatisticsCache(IServiceProvider serviceProvider);

    public int RecordTotal { get; private set; }

    public ImmutableArray<AvatarView> AvatarAppearances { get; private set; }

    public ValueTask InitializeForRoleCombatViewAsync(kyxsanRoleCombatStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<RoleCombatViewModel, kyxsanRoleCombatStatisticsMetadataContext>(context, AvatarAppearancesAsync);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearancesAsync(kyxsanRoleCombatStatisticsMetadataContext context)
    {
        RoleCombatStatisticsItem raw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IkyxsanRoleCombatService kyxsanService = scope.ServiceProvider.GetRequiredService<IkyxsanRoleCombatService>();
            raw = await kyxsanService.GetRoleCombatStatisticsItemAsync().ConfigureAwait(false);
        }

        RecordTotal = raw.RecordTotal;
        AvatarAppearances = [.. CurrentJoinLast(raw.BackupAvatarRates.EmptyIfDefault().OrderByDescending(ir => ir.Rate), default, data => data.Item, (data, dataLast) => new AvatarView(context.GetAvatar(data.Item), data.Rate, dataLast?.Rate))];
    }
}