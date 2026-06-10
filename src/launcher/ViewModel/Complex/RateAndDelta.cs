//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.ViewModel.Complex;

internal class RateAndDelta
{
    public RateAndDelta(double rate, double? lastRate)
    {
        Rate = $"{rate:P3}";
        RateDelta = lastRate.TryGetValue(out double lastRateValue) ? FormatDelta(rate - lastRateValue) : string.Empty;
    }

    public string Rate { get; }

    public string RateDelta { get; }

    public static RateAndDelta Create((double Rate, double? LastRate) tuple)
    {
        return new RateAndDelta(tuple.Rate, tuple.LastRate);
    }

    public static RateAndDelta Create((double Rate, double LastRate) tuple)
    {
        return new RateAndDelta(tuple.Rate, tuple.LastRate);
    }

    private static string FormatDelta(double value)
    {
        return value switch
        {
            >= 0 => $"+{value:P3}",
            < 0 => $"{value:P3}",
            _ => string.Empty,
        };
    }
}