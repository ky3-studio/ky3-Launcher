//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model;

/// <summary>
/// 封装带有名称描述的值
/// 在绑定枚举变量时非常有用
/// https://github.com/microsoft/microsoft-ui-xaml/issues/4266
/// 直接绑定枚举变量会显示 Windows.Foundation.IReference{T}
/// </summary>
/// <typeparam name="T">包含值的类型</typeparam>
internal class NameValue<T> : IEquatable<NameValue<T>>
{
    public NameValue(string name, T value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public T Value { get; }

    public bool Equals(NameValue<T>? other)
    {
        return other is not null
            && string.Equals(Name, other.Name)
            && EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Value);
    }
}