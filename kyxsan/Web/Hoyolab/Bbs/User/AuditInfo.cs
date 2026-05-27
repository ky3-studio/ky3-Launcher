//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Bbs.User;

internal sealed class AuditInfo
{
    [JsonPropertyName("is_nickname_in_audit")]
    public bool IsNicknameInAudit { get; set; }

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("is_introduce_in_audit")]
    public bool IsIntroduceInAudit { get; set; }

    [JsonPropertyName("introduce")]
    public string Introduce { get; set; } = default!;

    [JsonPropertyName("nickname_status")]
    public int NicknameStatus { get; set; }
}