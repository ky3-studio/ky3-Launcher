//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Text;

namespace kyxsan.Extension;

internal static class StringBuilderExtension
{
    extension(StringBuilder sb)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringBuilder AppendIf(bool condition, char? value)
        {
            return condition ? sb.Append(value) : sb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringBuilder AppendIf(bool condition, string? value)
        {
            return condition ? sb.Append(value) : sb;
        }

        public string ToStringTrimEndNewLine()
        {
            int length = sb.Length;
            int index = length - 1;

            while (index >= 0 && (char.IsWhiteSpace(sb[index]) || sb[index] == '\n' || sb[index] == '\r'))
            {
                index--;
            }

            return index < 0 ? string.Empty : sb.ToString(0, index + 1);
        }
    }
}