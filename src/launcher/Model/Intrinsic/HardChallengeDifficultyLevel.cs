//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model.Intrinsic;

[ExtendedEnum]
internal enum HardChallengeDifficultyLevel
{
    None = 0,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelNormal))]
    Normal = 1,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelAdvancing))]
    Advancing = 2,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelHard))]
    Hard = 3,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelMenacing))]
    Menacing = 4,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelFearless))]
    Fearless = 5,

    [LocalizationKey(nameof(SH.ModelIntrinsicHardChallengeDifficultyLevelDire))]
    Dire = 6,
}