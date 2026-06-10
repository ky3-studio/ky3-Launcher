//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.LifeCycle.InterProcess.Yae;

internal enum InterestedPropType
{
    None = 0,
    PlayerHCoin = 10015,
    PlayerSCoin = 10016,
    PlayerWaitSubHCoin = 10022,
    PlayerWaitSubSCoin = 10023,
    PlayerMCoin = 10025,
    PlayerWaitSubMCoin = 10026,
    PlayerHomeCoin = 10042,
    PlayerWaitSubHomeCoin = 10043,
    PlayerRoleCombatCoin = 10053,
    PlayerMusicGameBookCoin = 10058,
}