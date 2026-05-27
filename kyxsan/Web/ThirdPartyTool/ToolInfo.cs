namespace kyxsan.Web.ThirdPartyTool;

internal sealed class ToolInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("files")]
    public List<string> Files { get; set; } = default!;
}