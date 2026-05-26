//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics;

namespace kyxsan.UI.Xaml.Control.Layout;

[DebuggerDisplay("Count = {Count}, Height = {Height}")]
internal sealed partial class UniformStaggeredColumnLayout : List<UniformStaggeredItem>
{
    public double Height { get; private set; }

    public new void Add(UniformStaggeredItem item)
    {
        Height = item.Top + item.Height;
        base.Add(item);
    }

    public new void Clear()
    {
        Height = 0;
        base.Clear();
    }
}