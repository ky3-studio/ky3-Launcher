//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Bbs.User;

internal sealed class UserFullInfoWrapper
{
    [JsonPropertyName("user_info")]
    public UserInfo UserInfo { get; set; } = default!;

    [JsonPropertyName("follow_relation")]
    public JsonElement? FollowRelation { get; set; }

    [JsonPropertyName("auth_relations")]
    public List<JsonElement> AuthRelations { get; set; } = default!;

    [JsonPropertyName("is_in_blacklist")]
    public bool IsInBlacklist { get; set; }

    [JsonPropertyName("is_has_collection")]
    public bool IsHasCollection { get; set; }

    [JsonPropertyName("is_creator")]
    public bool IsCreator { get; set; }

    [JsonPropertyName("customer_service")]
    public CustomerService CustomerService { get; set; } = default!;

    [JsonPropertyName("audit_info")]
    public AuditInfo AuditInfo { get; set; } = default!;
}