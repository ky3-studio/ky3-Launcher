using kyxsan.Web.ThirdPartyTool;
using System.Collections.Immutable;

namespace kyxsan.Service.ThirdPartyTool;

internal interface IThirdPartyToolService
{
    ValueTask<ImmutableArray<ToolInfo>> GetToolsAsync(CancellationToken token = default);

    ValueTask<bool> DownloadToolAsync(ToolInfo tool, IProgress<double>? progress = null, CancellationToken token = default);

    ValueTask<bool> LaunchToolAsync(ToolInfo tool);

    bool IsToolDownloaded(ToolInfo tool);
}