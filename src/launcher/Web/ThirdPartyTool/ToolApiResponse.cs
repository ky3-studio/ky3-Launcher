using System.Collections.Immutable;

namespace Launcher.Web.ThirdPartyTool;

internal sealed class ToolApiResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    [JsonPropertyName("data")]
    public ImmutableArray<ToolInfo> Data { get; set; } = ImmutableArray<ToolInfo>.Empty;
}
