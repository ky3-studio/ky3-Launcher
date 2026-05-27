//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.GachaLog;

internal sealed partial class HistoryWish : Wish, IPropertyValuesProvider
{
    public required string Version { get; init; }

    public required Uri BannerImage { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeUpList { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleUpList { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeList { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleList { get; init; }

    public required ImmutableArray<StatisticsItem> BlueList { get; init; }
}
