//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;

namespace kyxsan.ViewModel.GachaLog;

internal sealed class Countdown
{
    public Countdown(Item item)
    {
        Item = item;
    }

    public string FormattedLastTime
    {
        get => LastTime <= DateTimeOffset.Now ? SH.FormatViewModelGachaLogCountdownLastTime(LastTime) : SH.ViewModelGachaLogCountdownCurrentWish;
    }

    public string FormattedVersionOrder { get => Histories.First().FormattedVersionOrder; }

    public string FormattedCountdown
    {
        get
        {
            int cdDays = (int)(DateTimeOffset.Now - LastTime).TotalDays;
            return cdDays > 0 ? SH.FormatViewModelGachaLogCountdownLastTimeDelta(cdDays) : SH.FormatViewModelGachaLogCountdownCurrentWishDelta(-cdDays);
        }
    }

    public string FormattedHistoryCount { get => SH.FormatViewModelGachaLogCountdownHistoryCount(Histories.Count); }

    public Item Item { get; }

    public Uri? NameCardPic { get; set; }

    public List<CountdownHistory> Histories { get; } = [];

    public Uri? LatestBanner { get => Histories.FirstOrDefault()?.Banner; }

    internal DateTimeOffset LastTime { get => Histories.FirstOrDefault()?.LastTime ?? default; }
}
