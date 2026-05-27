//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Weapon;

[JsonConverter(typeof(Converter))]
internal sealed class WeaponTypeValueCollection
{
    private readonly SortedDictionary<FightProperty, GrowCurveType> typeValues = [];
    private readonly SortedDictionary<FightProperty, float> typeInitValues = [];

    public WeaponTypeValueCollection(ImmutableArray<WeaponTypeValue> array)
    {
        Array = array;
        foreach (ref readonly WeaponTypeValue entry in array.AsSpan())
        {
            typeValues.Add(entry.Type, entry.Value);
            typeInitValues.Add(entry.Type, entry.InitValue);
        }
    }

    internal ImmutableArray<WeaponTypeValue> Array { get; }

    internal IReadOnlyDictionary<FightProperty, GrowCurveType> TypeValues { get => typeValues; }

    internal IReadOnlyDictionary<FightProperty, float> TypeInitValues { get => typeInitValues; }
}

[SuppressMessage("", "SA1402")]
file sealed class Converter : JsonConverter<WeaponTypeValueCollection>
{
    public override WeaponTypeValueCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<ImmutableArray<WeaponTypeValue>>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, WeaponTypeValueCollection value, JsonSerializerOptions options)
    {
        throw new JsonException();
    }
}