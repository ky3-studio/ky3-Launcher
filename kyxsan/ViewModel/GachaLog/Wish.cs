//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.ViewModel.GachaLog;

internal abstract class Wish
{
    public required string Name { get; init; }

    public required int TotalCount { get; init; }

    public string FormattedTimeSpan
    {
        get
        {
            if (From == DateTimeOffset.MaxValue && To == DateTimeOffset.MinValue)
            {
                return string.Empty;
            }

            return $"{From:yyyy.MM.dd} - {To:yyyy.MM.dd}";
        }
    }

    public string FormattedTotalCount
    {
        get => SH.FormatModelBindingGachaWishBaseTotalCount(TotalCount);
    }

    internal required DateTimeOffset From { get; init; }

    internal required DateTimeOffset To { get; init; }
}
