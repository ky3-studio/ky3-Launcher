//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Bbs.User;

internal sealed class UserInfo : UserCommon
{
    [JsonPropertyName("level_exps")]
    public ImmutableArray<LevelExperience> LevelExps { get; init; }

    [JsonPropertyName("achieve")]
    public Achieve? Achieve { get; init; }

    [JsonPropertyName("community_info")]
    public CommunityInfo? CommunityInfo { get; init; }

    [JsonPropertyName("is_logoff")]
    public bool IsLogOff { get; init; }

    [JsonPropertyName("ip_region")]
    public string? IpRegion { get; init; }

    [JsonPropertyName("show_text")]
    public string? ShowText { get; init; }

    [JsonPropertyName("avatar_ext")]
    public AvatarExtend? AvatarExtend { get; init; }

    [JsonIgnore]
    public Uri AvatarUri
    {
        get
        {
            Uri? avatarUrl = AvatarExtend?.HdResources.SingleOrDefault(r => r.Format == AvatarExtend.AvatarType) is { Url: { } urlV2 }
                ? urlV2
                : AvatarUrl;

            string? source = avatarUrl?.OriginalString;
            if (avatarUrl is not null && !string.IsNullOrEmpty(source))
            {
                return avatarUrl;
            }

            string target = string.IsNullOrEmpty(IpRegion)
                    ? $"https://img-os-static.hoyolab.com/avatar/avatar{Avatar}.png"
                    : $"https://bbs-static.miyoushe.com/avatar/avatar{Avatar}.png";

            return target.ToUri();
        }
    }
}