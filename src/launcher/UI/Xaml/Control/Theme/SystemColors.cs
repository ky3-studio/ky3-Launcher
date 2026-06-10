//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Windows.UI;

namespace kyxsan.UI.Xaml.Control.Theme;

internal static class SystemColors
{
    public static Color BaseLowColor(bool isDarkMode)
    {
        return isDarkMode ? ColorHelper.ToColor(0x33FFFFFF) : ColorHelper.ToColor(0x33000000);
    }

    public static Color BaseMediumLowColor(bool isDarkMode)
    {
        return isDarkMode ? ColorHelper.ToColor(0x66FFFFFF) : ColorHelper.ToColor(0x66000000);
    }

    public static Color BaseHighColor(bool isDarkMode)
    {
        return isDarkMode ? ColorHelper.ToColor(0xFFFFFFFF) : ColorHelper.ToColor(0xFF000000);
    }
}
