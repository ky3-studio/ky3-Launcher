using Launcher.Web.ThirdPartyTool;
using System.Collections.Immutable;

namespace Launcher.Service.ThirdPartyTool;

internal interface IThirdPartyToolService
{
    ValueTask<ImmutableArray<ToolInfo>> GetToolsAsync(CancellationToken token = default);

    ValueTask<bool> DownloadToolAsync(ToolInfo tool, IProgress<double>? progress = null, CancellationToken token = default);

    ValueTask<bool> LaunchToolAsync(ToolInfo tool);

    bool IsToolDownloaded(ToolInfo tool);
}
