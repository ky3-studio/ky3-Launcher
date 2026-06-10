//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.RoleCombat;

internal sealed class SplendourBuffView
{
    private SplendourBuffView(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        Icon = roleCombatSplendourBuff.Icon.ToUri();
        Level = roleCombatSplendourBuff.Level;
        Name = roleCombatSplendourBuff.Name;

        Effects = roleCombatSplendourBuff.LevelEffects.SelectAsArray(static e => e.Description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase));
    }

    public Uri Icon { get; }

    public int Level { get; }

    public string Name { get; }

    public ImmutableArray<string> Effects { get; }

    public static SplendourBuffView Create(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        return new(roleCombatSplendourBuff);
    }
}