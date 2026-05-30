//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace kyxsan.Model.Metadata.Converter;

internal static partial class CommonNameExtractor
{
    [GeneratedRegex("^UI_(.*)$")]
    private static partial Regex UI { get; }

    [GeneratedRegex("^UI_AvatarIcon_(.*)$")]
    private static partial Regex UIAvatarIcon { get; }

    public static string ExtractUIAvatarIconName(string name)
    {
        return UIAvatarIcon.Match(name).Groups[1].Value;
    }

    public static string ExtractUIName(string name)
    {
        return UI.Match(name).Groups[1].Value;
    }
}