//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace kyxsan.UI.Xaml.Media.Animation;

internal static class Constants
{
    public const string Zero = "0";

    public const string One = "1";

    public const string OnePointOne = "1.1";

    public static readonly TimeSpan ImageZoom = TimeSpan.FromSeconds(0.5);

    public static readonly TimeSpan ImageScaleFadeIn = TimeSpan.FromSeconds(0.3);

    public static readonly TimeSpan ImageScaleFadeOut = TimeSpan.FromSeconds(0.2);

    public static readonly TimeSpan ImageOpacityFadeInOut = TimeSpan.FromSeconds(1);

    public static readonly TimeSpan ImageOpacityFadeInOutFast = TimeSpan.FromSeconds(0.2);

    public static readonly GridLength ZeroGridLength = new(0);
}