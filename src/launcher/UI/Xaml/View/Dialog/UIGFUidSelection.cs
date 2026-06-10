//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.View.Dialog;

internal sealed partial class UIGFUidSelection
{
    public UIGFUidSelection(uint uid)
    {
        Uid = uid;
    }

    public uint Uid { get; }

    public static UIGFUidSelection Create(uint uid)
    {
        return new(uid);
    }

    public static ImmutableArray<uint> GetSelectedUidArray(ListViewBase listView)
    {
        return [.. listView.SelectedItems.Cast<UIGFUidSelection>().Select(static data => data.Uid)];
    }
}