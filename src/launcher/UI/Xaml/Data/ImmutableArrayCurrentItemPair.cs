//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.Data;

internal partial class ImmutableArrayCurrentItemPair<T> : ObservableObject
{
    public ImmutableArrayCurrentItemPair(ImmutableArray<T> array)
    {
        Array = array;
    }

    public ImmutableArrayCurrentItemPair(ImmutableArray<T> array, T? currentItem)
        : this(array)
    {
        CurrentItem = currentItem;
    }

    public ImmutableArray<T> Array { get; }

    [ObservableProperty]
    public partial T? CurrentItem { get; set; }
}