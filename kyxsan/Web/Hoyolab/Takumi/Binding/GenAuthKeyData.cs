//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Globalization;

namespace kyxsan.Web.Hoyolab.Takumi.Binding;

internal sealed class GenAuthKeyData
{
    public GenAuthKeyData(string authAppId, string gameBiz, PlayerUid uid)
    {
        AuthAppId = authAppId;
        GameBiz = gameBiz;
        GameUid = int.Parse(uid.Value, CultureInfo.InvariantCulture);
        Region = uid.Region;
    }

    [JsonPropertyName("auth_appid")]
    public string AuthAppId { get; set; }

    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; }

    [JsonPropertyName("game_uid")]
    public int GameUid { get; set; }

    [JsonPropertyName("region")]
    public Region Region { get; set; }

    public static GenAuthKeyData CreateForWebViewGacha(PlayerUid uid)
    {
        return new("webview_gacha", "hk4e_cn", uid);
    }
}