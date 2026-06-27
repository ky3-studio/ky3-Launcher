//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

namespace Launcher.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 实时便笺小组件数据
/// </summary>
internal sealed class WidgetDailyNote : DailyNoteCommon
{
    /// <summary>
    /// 是否签到
    /// </summary>
    [JsonPropertyName("has_signed")]
    public bool HasSigned { get; set; }

    /// <summary>
    /// 签到页面链接
    /// </summary>
    [JsonPropertyName("sign_url")]
    public Uri SignUrl { get; set; } = default!;
}