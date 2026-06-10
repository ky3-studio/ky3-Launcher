//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Text.Json.Annotation;
using System.Text.Json.Serialization.Metadata;

namespace kyxsan.Core.Text.Json;

internal static class JsonTypeInfoResolvers
{
    private static readonly Type JsonEnumHandlingAttributeType = typeof(JsonEnumHandlingAttribute);

    public static void ResolveEnumType(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind is not JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            if (!property.PropertyType.IsEnum)
            {
                continue;
            }

            if (property.AttributeProvider is not { } provider)
            {
                continue;
            }

            if (provider.GetCustomAttributes(JsonEnumHandlingAttributeType, false) is [JsonEnumHandlingAttribute attr])
            {
                property.CustomConverter = attr.CreateConverter(property);
            }
        }
    }
}