//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

[ExtendedEnum]
internal enum HardChallengeBestAvatarType
{
    None,

    /// <summary>
    /// UI_ACTIVITY_LEYLINEC_RECORD_ONEATTACK
    /// </summary>
    [LocalizationKey(nameof(SH.WebHardChallengeBestAvatarTypeOneAttack))]
    OneAttack = 1,

    /// <summary>
    /// UI_ACTIVITY_LEYLINEC_RECORD_ALLATTACK
    /// </summary>
    [LocalizationKey(nameof(SH.WebHardChallengeBestAvatarTypeAllAttack))]
    AllAttack = 2,
}