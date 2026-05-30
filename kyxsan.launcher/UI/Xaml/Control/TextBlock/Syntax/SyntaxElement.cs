//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.Control.TextBlock.Syntax;

internal abstract class SyntaxElement<TSelf>
    where TSelf : SyntaxElement<TSelf>
{
    protected SyntaxElement(TextPosition position, ImmutableArray<TSelf> children)
    {
        Position = position;
        Children = children;
    }

    public ImmutableArray<TSelf> Children { get; }

    public TextPosition Position { get; }

    public ReadOnlySpan<char> GetSpan(ReadOnlySpan<char> source)
    {
        return source.Slice(Position.Start, Position.Length);
    }
}