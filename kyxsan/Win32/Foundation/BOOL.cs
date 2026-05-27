//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly struct BOOL
{
    public static readonly BOOL FALSE = 0;
    public static readonly BOOL TRUE = 1;

    public readonly int Value;

    public BOOL(bool value)
    {
        Value = value ? 1 : 0;
    }

    public static unsafe implicit operator int(BOOL value)
    {
        return *(int*)&value;
    }

    public static unsafe implicit operator BOOL(int value)
    {
        return *(BOOL*)&value;
    }

    public static implicit operator BOOL(bool value)
    {
        return new(value);
    }

    public static implicit operator bool(BOOL value)
    {
        return value != 0;
    }
}