//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;

namespace Launcher.Model.Metadata;

internal class BaseValue
{
    public required float HpBase { get; init; }

    public required float AttackBase { get; init; }

    public required float DefenseBase { get; init; }

    public virtual float GetValue(FightProperty fightProperty)
    {
        return fightProperty switch
        {
            FightProperty.FIGHT_PROP_BASE_HP => HpBase,
            FightProperty.FIGHT_PROP_BASE_ATTACK => AttackBase,
            FightProperty.FIGHT_PROP_BASE_DEFENSE => DefenseBase,
            _ => 0,
        };
    }
}
