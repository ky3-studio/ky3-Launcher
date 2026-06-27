//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

namespace Launcher.Web.Hoyolab.Bbs.User;

internal sealed class LevelExperience
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("exp")]
    public int Exp { get; set; }

    /// <summary>
    /// 游戏Id
    /// 1 大别野
    /// 2 原神
    /// 3 崩坏学园2
    /// 4 未定事件簿
    /// 5 崩坏：星穹铁道
    /// 6 绝区零
    /// 8 崩坏3
    /// </summary>
    [JsonPropertyName("game_id")]
    public int GameId { get; set; }
}
