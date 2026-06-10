//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.IO;

internal readonly struct ValueDirectory : IEquatable<ValueDirectory>
{
    private readonly string value;

    private ValueDirectory(string value)
    {
        this.value = value;
    }

    public bool HasValue { get => value is not null; }

    public static implicit operator string(ValueDirectory value)
    {
        return value.value;
    }

    public static implicit operator ValueDirectory(string? value)
    {
        return new(value!);
    }

    [SuppressMessage("", "CA1307")]
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        return value;
    }

    public bool Equals(ValueDirectory other)
    {
        return string.Equals(value, other.value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueDirectory other && Equals(other);
    }
}