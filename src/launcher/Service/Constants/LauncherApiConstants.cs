//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

namespace Launcher.Service.Constants;

internal static class LauncherApiConstants
{
    internal const int DefaultTimeoutSeconds = 10;
    internal const int DownloadTimeoutSeconds = 20;
    internal const int ImageDownloadTimeoutSeconds = 15;
    internal const int UpdateCheckIntervalMinutes = 30;
    internal const int BannerRotationSeconds = 5;
    internal const int GameProcessCheckSeconds = 1;
    internal const int BackgroundSwitchDefaultSeconds = 8;

    internal const string MiHoYoGameInfoApi = "https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id=jGHBHlcOq1&language=zh-cn";
    internal const string MiHoYoGameContentApi = "https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getGameContent?launcher_id=jGHBHlcOq1&game_id=1Z8W5NHUQb&language=zh-cn";
}
