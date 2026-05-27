//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using kyxsan.Core.Text.Json;

namespace kyxsan.Model.Entity.Configuration;

internal sealed class JsonTextValueConverter<TPropertyType> : ValueConverter<TPropertyType, string>
{
    [SuppressMessage("", "SH007")]
    public JsonTextValueConverter()
        : base(
            static obj => JsonSerializer.Serialize(obj, JsonOptions.Default),
            static str => string.IsNullOrEmpty(str) ? default! : JsonSerializer.Deserialize<TPropertyType>(str, JsonOptions.Default)!)
    {
    }
}