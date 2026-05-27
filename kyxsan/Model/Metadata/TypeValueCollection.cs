//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Model.Metadata;

[JsonConverter(typeof(ConverterFactory))]
internal sealed class TypeValueCollection<TType, TValue>
    where TType : notnull
{
    private readonly SortedDictionary<TType, TValue> typeValues = [];

    public TypeValueCollection(ImmutableArray<TypeValue<TType, TValue>> array)
    {
        Array = array;
        foreach (ref readonly TypeValue<TType, TValue> entry in array.AsSpan())
        {
            typeValues.Add(entry.Type, entry.Value);
        }
    }

    internal ImmutableArray<TypeValue<TType, TValue>> Array { get; }

    internal IReadOnlyDictionary<TType, TValue> TypeValues { get => typeValues; }

    public TValue? GetValueOrDefault(TType type)
    {
        return typeValues.GetValueOrDefault(type);
    }
}

[SuppressMessage("", "SA1402")]
file sealed class ConverterFactory : JsonConverterFactory
{
    private static readonly Type ConverterType = typeof(Converter<,>);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(TypeValueCollection<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return Activator.CreateInstance(ConverterType.MakeGenericType(typeToConvert.GetGenericArguments())) as JsonConverter;
    }
}

[SuppressMessage("", "SA1402")]
file sealed class Converter<TType, TValue> : JsonConverter<TypeValueCollection<TType, TValue>>
    where TType : notnull
{
    public override TypeValueCollection<TType, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<ImmutableArray<TypeValue<TType, TValue>>>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, TypeValueCollection<TType, TValue> value, JsonSerializerOptions options)
    {
        throw new JsonException();
    }
}