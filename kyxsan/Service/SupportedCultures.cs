//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using System.Collections.Immutable;
using System.Globalization;

namespace kyxsan.Service;

[SuppressMessage("", "SA1500")]
[SuppressMessage("", "SA1513")]
internal static class SupportedCultures
{
    public static ImmutableArray<NameCultureInfoValue> GetValues()
    {
        return
        [
            ToNameValue(CultureInfo.GetCultureInfo("zh-Hans"), LocalizationSource.kyxsan),
        ];
    }

    private static NameCultureInfoValue ToNameValue(CultureInfo info, LocalizationSource source)
    {
        return new(info.NativeName, info, source);
    }
}