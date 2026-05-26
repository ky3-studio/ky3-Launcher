//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;

namespace kyxsan.Service.GachaLog.Factory;

internal sealed class GachaStatisticsItemCounter
{
    public GachaStatisticsItemCounter(GachaStatisticsFactoryContext context)
    {
        OrangeAvatar = [];
        PurpleAvatar = [];
        OrangeWeapon = [];
        PurpleWeapon = [];
        BlueWeapon = [];
    }

    public Dictionary<Avatar, int> OrangeAvatar { get; }

    public Dictionary<Avatar, int> PurpleAvatar { get; }

    public Dictionary<Weapon, int> OrangeWeapon { get; }

    public Dictionary<Weapon, int> PurpleWeapon { get; }

    public Dictionary<Weapon, int> BlueWeapon { get; }
}
