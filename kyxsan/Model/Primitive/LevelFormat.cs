//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace kyxsan.Model.Primitive;

internal static class LevelFormat
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(uint value)
    {
        return $"Lv. {value}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(uint value, uint extra)
    {
        return extra > 0 ? $"Lv. {value + extra} ({value} +{extra})" : $"Lv. {value}";
    }
}