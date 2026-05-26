using kyxsan.Core.Text.Json.Annotation;
using kyxsan.Model.Intrinsic;

namespace kyxsan.Web.Hoyolab.Hk4e.Event.GachaInfo;

internal sealed class GachaLogItem
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("gacha_type")]
    [JsonEnumHandling(JsonEnumHandling.NumberString)]
    public GachaType GachaType { get; set; }

    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = default!;

    [JsonPropertyName("count")]
    public string Count { get; set; } = default!;

    [JsonPropertyName("time")]
    public string Time { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("lang")]
    public string Language { get; set; } = default!;

    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = default!;

    [JsonPropertyName("rank_type")]
    public string RankType { get; set; } = default!;

    [JsonPropertyName("id")]
    public long Id { get; set; }
}
