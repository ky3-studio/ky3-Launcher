using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Hk4e.Event.GachaInfo;

internal sealed class GachaLogPage
{
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;

    [JsonPropertyName("size")]
    public string Size { get; set; } = default!;

    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    [JsonPropertyName("list")]
    public ImmutableArray<GachaLogItem> List { get; set; }
}
