//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Bbs.Home;

internal sealed class NewHomeNewInfo
{
    [JsonPropertyName("navigator")]
    public required ImmutableArray<AppNavigator> Navigator { get; init; }

    [JsonPropertyName("discussion")]
    public required Discussion Discussion { get; init; }

    [JsonPropertyName("background")]
    public required Background Background { get; init; }

    [JsonPropertyName("official")]
    public required Official Official { get; init; }

    [JsonPropertyName("carousels")]
    public required Carousels Carousels { get; init; }

    [JsonPropertyName("hot_topics")]
    public required JsonElement HotTopics { get; init; }

    [JsonPropertyName("game_receptions")]
    public required ImmutableArray<GameReception> GameReceptions { get; init; }

    [JsonPropertyName("posts")]
    public required ImmutableArray<JsonElement> Posts { get; init; }

    [JsonPropertyName("lives")]
    public required ImmutableArray<Live> Lives { get; init; }

    [JsonPropertyName("recommend_villa")]
    public required JsonElement RecommendVilla { get; init; }

    [JsonPropertyName("image_post_card")]
    public required ImmutableArray<JsonElement> ImagePostCard { get; init; }

    [JsonPropertyName("link_card")]
    public required ImmutableArray<JsonElement> LinkCard { get; init; }

    [JsonPropertyName("album_card")]
    public required ImmutableArray<JsonElement> AlbumCard { get; init; }

    [JsonPropertyName("is_resource_unchanged")]
    public required bool IsResourceUnchanged { get; init; }

    [JsonPropertyName("exposed_resource_tickets")]
    public required ImmutableArray<string> ExposedResourceTickets { get; init; }
}