//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.kyxsan.SpiralAbyss.Converter;

internal sealed class ReliquarySetsConverter : JsonConverter<ReliquarySets>
{
    public override ReliquarySets? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } source)
        {
            List<ReliquarySet> sets = [];
            ReadOnlySpan<char> sourceSpan = source.AsSpan();
            foreach (Range range in sourceSpan.Split(','))
            {
                ReadOnlySpan<char> target = sourceSpan[range];
                if (!target.IsEmpty)
                {
                    sets.Add(new(target));
                }
            }

            return new(sets);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, ReliquarySets value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(',', value));
    }
}