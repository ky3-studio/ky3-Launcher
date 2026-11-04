namespace Launcher.Service.RemoteConfig;

internal static class BackendApiRoutes
{
    internal const string ServerRoot = "https://8.134.75.17:9000";
    internal const string ApiBase = $"{ServerRoot}/api";
    internal const string StaticBase = "http://8.134.75.17";

    internal const string GitHubBase = "https://github.com/ky3-git/ky3-Launcher";
    internal const string GitHubIssues = $"{GitHubBase}/issues/new/choose";
    internal const string GitHubPulls = $"{GitHubBase}/pulls";
}
