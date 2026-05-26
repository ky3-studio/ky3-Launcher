//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace kyxsan.Service.Notification;

internal static partial class TextReadingTime
{
    [GeneratedRegex("""\p{IsCJKUnifiedIdeographs}""")]
    private static partial Regex ChineseRegex { get; }

    [GeneratedRegex("""\b[A-Za-z]+\b""")]
    private static partial Regex EnglishRegex { get; }

    public static int Estimate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        double chineseSeconds = (double)ChineseRegex.Matches(text).Count / 250 * 60;
        double englishSeconds = (double)EnglishRegex.Matches(text).Count / 200 * 60;

        return (int)Math.Ceiling((chineseSeconds + englishSeconds) * 1000);
    }
}