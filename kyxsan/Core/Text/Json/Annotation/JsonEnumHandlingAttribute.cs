//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Text.Json.Converter;
using System.Text.Json.Serialization.Metadata;

namespace kyxsan.Core.Text.Json.Annotation;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class JsonEnumHandlingAttribute : Attribute
{
    private static readonly Type UnsafeEnumConverterType = typeof(UnsafeEnumConverter<>);

    private readonly JsonEnumHandling readAs;
    private readonly JsonEnumHandling writeAs;

    public JsonEnumHandlingAttribute(JsonEnumHandling readAndWriteAs)
    {
        readAs = readAndWriteAs;
        writeAs = readAndWriteAs;
    }

    public JsonEnumHandlingAttribute(JsonEnumHandling readAs, JsonEnumHandling writeAs)
    {
        this.readAs = readAs;
        this.writeAs = writeAs;
    }

    internal JsonConverter CreateConverter(JsonPropertyInfo info)
    {
        Type converterType = UnsafeEnumConverterType.MakeGenericType(info.PropertyType);
        JsonConverter? converter = Activator.CreateInstance(converterType, readAs, writeAs) as JsonConverter;
        ArgumentNullException.ThrowIfNull(converter);
        return converter;
    }
}