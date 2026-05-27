//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab;

/// <summary>
/// 键部分
/// </summary>
[SuppressMessage("", "SA1310")]
internal sealed partial class Cookie
{
    public const string LOGIN_TICKET = "login_ticket";
    public const string LOGIN_UID = "login_uid";

    public const string ACCOUNT_ID = "account_id";
    public const string COOKIE_TOKEN = "cookie_token";

    public const string LTOKEN = "ltoken";
    public const string LTUID = "ltuid";

    public const string MID = "mid";
    public const string STOKEN = "stoken";
    public const string STUID = "stuid";
}
