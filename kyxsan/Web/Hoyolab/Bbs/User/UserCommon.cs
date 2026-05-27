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

internal class UserCommon
{
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; init; }

    [JsonPropertyName("introduce")]
    public string? Introduce { get; init; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }

    [JsonPropertyName("gender")]
    public UserGender Gender { get; init; }

    [JsonPropertyName("certification")]
    public Certification? Certification { get; init; }

    [JsonPropertyName("level_exp")]
    public LevelExperience? LevelExp { get; init; }

    [JsonPropertyName("avatar_url")]
    public Uri? AvatarUrl { get; init; }

    [JsonPropertyName("pendant")]
    public Uri? Pendant { get; init; }

    [JsonPropertyName("certifications")]
    public ImmutableArray<DetailedCertification> Certifications { get; init; }
}