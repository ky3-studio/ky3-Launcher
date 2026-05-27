//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.kyxsan.RoleCombat;

namespace kyxsan.Service.kyxsan;

[Service(ServiceLifetime.Scoped, typeof(IkyxsanRoleCombatService))]
internal sealed partial class kyxsanRoleCombatService : ObjectCacheService, IkyxsanRoleCombatService
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial kyxsanRoleCombatService(IServiceProvider serviceProvider);

    public override string TypeName { get; } = nameof(kyxsanRoleCombatService);

    public async ValueTask<RoleCombatStatisticsItem> GetRoleCombatStatisticsItemAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            kyxsanRoleCombatClient homaClient = scope.ServiceProvider.GetRequiredService<kyxsanRoleCombatClient>();
            return await FromCacheOrWebAsync(nameof(RoleCombatStatisticsItem), false, homaClient.GetStatisticsAsync).ConfigureAwait(false);
        }
    }
}