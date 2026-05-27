//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics;

namespace kyxsan.UI.Xaml.Control.TextBlock.Syntax;

[DebuggerDisplay("[{Start}..{End}]")]
internal readonly struct TextPosition
{
    public readonly int Start;
    public readonly int End;

    public TextPosition(int start, int end)
    {
        Start = start;
        End = end;
    }

    public readonly int Length
    {
        get => End - Start;
    }

    public TextPosition LeftShift(int offset)
    {
        return new(Start - offset, End - offset);
    }

    public TextPosition RightShift(int offset)
    {
        return new(Start + offset, End + offset);
    }
}