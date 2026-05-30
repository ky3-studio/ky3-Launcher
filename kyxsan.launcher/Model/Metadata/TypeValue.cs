//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model.Metadata;

internal class TypeValue<TType, TValue>
{
    public TypeValue(TType type, TValue value)
    {
        Type = type;
        Value = value;
    }

    public TType Type { get; }

    public TValue Value { get; }

    public override bool Equals(object? obj)
    {
        return obj is TypeValue<TType, TValue> value
            && EqualityComparer<TType>.Default.Equals(Type, value.Type)
            && EqualityComparer<TValue>.Default.Equals(Value, value.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Value);
    }
}