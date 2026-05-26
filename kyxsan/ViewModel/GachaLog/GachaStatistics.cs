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

internal sealed class GachaStatistics
{
    public required TypedWishSummary AvatarWish { get; init; }

    public required TypedWishSummary WeaponWish { get; init; }

    public required TypedWishSummary ChronicledWish { get; init; }

    public required TypedWishSummary StandardWish { get; init; }

    public required IAdvancedCollectionView<HistoryWish> HistoryWishes { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeAvatars { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleAvatars { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeWeapons { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleWeapons { get; init; }

    public required ImmutableArray<StatisticsItem> BlueWeapons { get; init; }
}
