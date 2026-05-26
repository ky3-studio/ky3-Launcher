//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Service.GachaLog.Factory;

internal readonly struct GachaStatisticsFactoryContext
{
    public readonly IServiceProvider ServiceProvider;
    public readonly ImmutableArray<Model.Entity.GachaItem> Items;
    public readonly ImmutableArray<HistoryWishBuilder> HistoryWishBuilders;
    public readonly GachaLogServiceMetadataContext Metadata;

    public GachaStatisticsFactoryContext(IServiceProvider serviceProvider, ImmutableArray<Model.Entity.GachaItem> items, ImmutableArray<HistoryWishBuilder> historyWishBuilders, GachaLogServiceMetadataContext metadata)
    {
        ServiceProvider = serviceProvider;
        Items = items;
        HistoryWishBuilders = historyWishBuilders;
        Metadata = metadata;
    }

    public bool IsEmptyHistoryWishVisible { get => false; }
}
