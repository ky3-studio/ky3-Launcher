//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Extension;

internal static class NumberExtension
{
    private static readonly KeyValuePair<string, int>[] RomanNumeralsSequence =
    [
        KeyValuePair.Create("M", 1000),
        KeyValuePair.Create("CM", 900),
        KeyValuePair.Create("D", 500),
        KeyValuePair.Create("CD", 400),
        KeyValuePair.Create("C", 100),
        KeyValuePair.Create("XC", 90),
        KeyValuePair.Create("L", 50),
        KeyValuePair.Create("XL", 40),
        KeyValuePair.Create("X", 10),
        KeyValuePair.Create("IX", 9),
        KeyValuePair.Create("V", 5),
        KeyValuePair.Create("IV", 4),
        KeyValuePair.Create("I", 1),
    ];

    extension(int input)
    {
        public string ToRoman()
        {
            const int MinValue = 1;
            const int MaxValue = 3999;
            const int MaxRomanNumeralLength = 15;

            if (input is < MinValue or > MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }

            Span<char> builder = stackalloc char[MaxRomanNumeralLength];
            int pos = 0;

            foreach (KeyValuePair<string, int> pair in RomanNumeralsSequence)
            {
                while (input >= pair.Value)
                {
                    pair.Key.AsSpan().CopyTo(builder[pos..]);
                    pos += pair.Key.Length;
                    input -= pair.Value;
                }
            }

            return builder[..pos].ToString();
        }
    }

    extension(in uint x)
    {
        public uint StringLength
        {
            get => (uint)(MathF.Log10(x) + 1);
        }
    }
}