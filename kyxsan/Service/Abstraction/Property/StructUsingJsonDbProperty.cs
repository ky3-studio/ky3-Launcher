//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Text.Json;

namespace kyxsan.Service.Abstraction.Property;

internal sealed partial class StructUsingJsonDbProperty<T> : StructUsingCustomDbProperty<T>
    where T : struct
{
    public StructUsingJsonDbProperty(IServiceProvider serviceProvider, string key, Func<T> defaultValueFactory)
        : base(serviceProvider, key, defaultValueFactory, From, To)
    {
    }

    public StructUsingJsonDbProperty(IServiceProvider serviceProvider, string key, T defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    private static T From(string source)
    {
        return JsonSerializer.Deserialize<T>(source, JsonOptions.Default);
    }

    private static string To(T value)
    {
        return JsonSerializer.Serialize(value, JsonOptions.Default);
    }
}