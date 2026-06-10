//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace kyxsan.Core;

internal static class UnsafeDateTimeOffset
{
    public static DateTimeOffset ParseDateTime(ReadOnlySpan<char> span, TimeSpan offset)
    {
        return new(DateTime.Parse(span, CultureInfo.InvariantCulture), offset);
    }

    public static DateTimeOffset FromUnixTimeRelaxed(long? timestamp, in DateTimeOffset defaultValue)
    {
        if (timestamp is not { } value)
        {
            return defaultValue;
        }

        return value switch
        {
            >= -62135596800L and <= 253402300799L => DateTimeOffset.FromUnixTimeSeconds(value),
            >= -62135596800000L and <= 253402300799999L => DateTimeOffset.FromUnixTimeMilliseconds(value),
            _ => defaultValue,
        };
    }

    [Pure]
    public static DateTimeOffset AdjustOffsetOnly(DateTimeOffset dateTimeOffset, in TimeSpan offset)
    {
        return new(GetPrivateDateTime(in dateTimeOffset), offset);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dateTime")]
    private static extern ref DateTime GetPrivateDateTime(ref readonly DateTimeOffset dateTimeOffset);
}