//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace kyxsan.Extension;

internal static class StringExtension
{
    extension(string value)
    {
        public bool EqualsAny(ReadOnlySpan<string> values, StringComparison stringComparison)
        {
            foreach (ref readonly string item in values)
            {
                if (value.Equals(item, stringComparison))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Uri ToUri()
        {
            return new(value, UriKind.RelativeOrAbsolute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string TrimEnd(string value1)
        {
            return value.AsSpan().TrimEnd(value1).ToString();
        }
    }
}