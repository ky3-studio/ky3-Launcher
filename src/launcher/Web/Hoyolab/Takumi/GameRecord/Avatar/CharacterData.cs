//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class CharacterData
{
    public CharacterData(PlayerUid uid)
    {
        Uid = uid.Value;
        Server = uid.Region;
    }

    public CharacterData(PlayerUid uid, ImmutableArray<AvatarId> characterIds)
        : this(uid)
    {
        CharacterIds = characterIds;
    }

    [JsonPropertyName("sort_type")]
    public uint SortType { get; } = 1;

    [JsonPropertyName("character_ids")]
    public ImmutableArray<AvatarId>? CharacterIds { get; }

    [JsonPropertyName("role_id")]
    public string Uid { get; }

    [JsonPropertyName("server")]
    public Region Server { get; }
}