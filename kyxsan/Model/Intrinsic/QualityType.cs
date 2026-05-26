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
internal enum QualityType
{
    /// <summary>
    /// 无
    /// </summary>
    QUALITY_NONE = 0,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityWhite))]
    QUALITY_WHITE = 1,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityGreen))]
    QUALITY_GREEN = 2,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityBlue))]
    QUALITY_BLUE = 3,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityPurple))]
    QUALITY_PURPLE = 4,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityOrange))]
    QUALITY_ORANGE = 5,

    [LocalizationKey(nameof(SH.ModelIntrinsicItemQualityRed))]
    QUALITY_ORANGE_SP = 105,
}