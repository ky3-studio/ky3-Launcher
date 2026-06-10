//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Abstraction;
using kyxsan.ViewModel.GachaLog;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace kyxsan.Service.GachaLog.Factory;

internal static class GachaStatisticsFactoryExtension
{
    private static readonly ConcurrentDictionary<string, Color> KnownColors = [];

    public static ImmutableArray<StatisticsItem> ToStatisticsImmutableArray<TKey>(this Dictionary<TKey, int> dict)
        where TKey : IStatisticsItemConvertible
    {
        return [.. dict
            .Where(kvp => kvp.Value > 0)
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value))];
    }

    public static void CompleteAdding(this List<SummaryItem> list, int guaranteeOrangeThreshold, bool isEventBanner)
    {
        bool isPreviousUp = true;

        foreach (SummaryItem item in list)
        {
            if (item.IsUp && !isPreviousUp)
            {
                item.IsGuarantee = true;
            }

            if (isEventBanner && !item.IsUp)
            {
                item.IsNotUp = true;
            }

            isPreviousUp = item.IsUp;
            item.Color = GetUniqueColorByName(item.Name);
            item.GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        }

        list.Reverse();
    }

    [SuppressMessage("", "IDE0057")]
    private static Color GetUniqueColorByName(string name)
    {
        if (KnownColors.TryGetValue(name, out Color color))
        {
            return color;
        }

        ReadOnlySpan<byte> codes = MD5.HashData(Encoding.UTF8.GetBytes(name));
        Color current = Color.FromArgb(255, codes.Slice(0, 5).Average(), codes.Slice(5, 5).Average(), codes.Slice(10, 5).Average());
        KnownColors.TryAdd(name, current);
        return current;
    }
}
