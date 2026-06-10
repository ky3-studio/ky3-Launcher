//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace kyxsan.ViewModel.RoleCombat;

internal sealed class BuffView
{
    private BuffView(RoleCombatBuff roleCombatBuff)
    {
        Icon = roleCombatBuff.Icon.ToUri();
        Name = roleCombatBuff.Name;
        Description = roleCombatBuff.Description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase);
    }

    public Uri Icon { get; }

    public string Name { get; }

    public string Description { get; }

    public static BuffView Create(RoleCombatBuff roleCombatBuff)
    {
        return new(roleCombatBuff);
    }
}