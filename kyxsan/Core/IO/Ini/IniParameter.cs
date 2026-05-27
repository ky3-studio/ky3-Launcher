//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.IO.Ini;

internal sealed class IniParameter : IniElement, IEquatable<IniParameter>
{
    public IniParameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }

    public string Value { get; }

    public IniParameter WithValue(string value, out bool changed)
    {
        if (Value == value)
        {
            changed = false;
            return this;
        }

        changed = true;
        return new(Key, value);
    }

    public override string ToString()
    {
        return $"{Key}={Value}";
    }

    public bool Equals(IniParameter? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Key == other.Key
            && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IniParameter);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Value);
    }
}