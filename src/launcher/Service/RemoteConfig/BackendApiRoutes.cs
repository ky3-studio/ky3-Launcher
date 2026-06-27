using System.Net.Http;

namespace Launcher.Service.RemoteConfig;

internal static class BackendApiRoutes
{
    internal const string ServerRoot = "https://8.134.75.17:9000";
    internal const string ApiBase = $"{ServerRoot}/api";
    internal const string StaticBase = "http://8.134.75.17";

    internal const string Announcements = $"{ApiBase}/announcements";
    internal const string Heartbeat = $"{ApiBase}/heartbeat";

    internal const string FeedbackImage = $"{ApiBase}/feedback-image";
    internal const string Feedback = $"{ApiBase}/feedback";
    internal const string FeedbackReplies = $"{ApiBase}/feedback/replies";
    internal const string FeedbackReplyRead = $"{ApiBase}/feedback/reply-read";

    internal const string GitHubBase = "https://github.com/ky3-git/ky3-Launcher";
    internal const string GitHubIssues = $"{GitHubBase}/issues/new/choose";
    internal const string GitHubPulls = $"{GitHubBase}/pulls";

    internal static HttpClient CreateHttpClient(TimeSpan timeout)
    {
        SocketsHttpHandler handler = new()
        {
            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true,
            },
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
        };
        return new HttpClient(handler) { Timeout = timeout };
    }
}
