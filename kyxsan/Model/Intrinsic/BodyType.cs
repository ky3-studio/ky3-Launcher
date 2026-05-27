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
internal enum BodyType
{
    BODY_NONE,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeBoy))]
    BODY_BOY,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeGirl))]
    BODY_GIRL,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeLady))]
    BODY_LADY,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeMale))]
    BODY_MALE,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeLoli))]
    BODY_LOLI,
}