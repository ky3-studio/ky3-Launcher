//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxColorElement : MiHoYoSyntaxElement
{
    public MiHoYoSyntaxColorElement(TextPosition fullPosition, TextPosition colorPosition, ImmutableArray<MiHoYoSyntaxElement> children)
        : base(fullPosition, children)
    {
        ColorPosition = colorPosition;
    }

    public TextPosition ColorPosition { get; }

    public ReadOnlySpan<char> GetColorSpan(ReadOnlySpan<char> source)
    {
        return source.Slice(ColorPosition.Start, ColorPosition.Length);
    }
}