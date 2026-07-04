// Launcher - App Announcement Service
// Fetches software announcements from the admin backend.

using Launcher.Core.Setting;
using System.Net.Http;

namespace Launcher.Service.RemoteConfig;

internal static class AppAnnouncementService
{
    internal static readonly HttpClient Http = BackendApiRoutes.CreateHttpClient(TimeSpan.FromSeconds(15));
    private static readonly object Lock = new();
    private static List<AppAnnouncement> _cached = [];
    private static Timer? _timer;
    private static bool _firstPoll = true;
    private static int _consecutiveFailures;

    private const int BaseIntervalSeconds = 60;
    private const int MaxIntervalSeconds = 300;

    private static HashSet<int> _seenIds = LoadSeenIds();
    private static readonly HashSet<int> _notifiedThisSession = [];

    public static event Action<List<AppAnnouncement>>? Changed;

    public static List<AppAnnouncement> Current
    {
        get { lock (Lock) { return [.. _cached]; } }
    }

    public static List<AppAnnouncement> FilterNew(List<AppAnnouncement> items)
    {
        lock (Lock)
        {
            List<AppAnnouncement> result = [];
            foreach (AppAnnouncement a in items)
            {
                if (!_seenIds.Contains(a.Id) && !_notifiedThisSession.Contains(a.Id))
                {
                    _notifiedThisSession.Add(a.Id);
                    result.Add(a);
                }
            }
            return result;
        }
    }

    public static void MarkAllSeen()
    {
        lock (Lock)
        {
            foreach (AppAnnouncement a in _cached)
                _seenIds.Add(a.Id);
            SaveSeenIds(_seenIds);
        }
    }

    public static async Task ForceRefreshAsync()
    {
        List<AppAnnouncement> latest = await FetchAsync().ConfigureAwait(false);
        bool fire;
        lock (Lock)
        {
            fire = !AreEqual(_cached, latest);
            if (fire) _cached = latest;
        }
        if (fire) Changed?.Invoke(latest);
    }

    private static HashSet<int> LoadSeenIds()
    {
        return new HashSet<int>(
            LocalSetting.Get(SettingKeys.AnnouncementSeenIds, "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out int id) ? id : -1)
                .Where(id => id >= 0));
    }

    private static void SaveSeenIds(HashSet<int> ids)
    {
        LocalSetting.Set(SettingKeys.AnnouncementSeenIds, string.Join(',', ids));
    }

    public static void StartPolling()
    {
        if (_timer is not null) return;
        _timer = new Timer(OnTimerTick, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
    }

    private static async void OnTimerTick(object? state)
    {
        List<AppAnnouncement> latest = await FetchAsync().ConfigureAwait(false);

        bool success = latest.Count > 0 || _cached.Count == 0;
        if (success)
        {
            Interlocked.Exchange(ref _consecutiveFailures, 0);
        }
        else
        {
            Interlocked.Increment(ref _consecutiveFailures);
        }

        bool fire;
        lock (Lock)
        {
            bool changed = !AreEqual(_cached, latest);
            fire = _firstPoll || changed;
            if (fire) _cached = latest;
            _firstPoll = false;
        }

        if (fire) Changed?.Invoke(latest);

        // Exponential backoff: 60s → 120s → 240s, cap at 300s
        int delay = Math.Min(BaseIntervalSeconds * (1 << _consecutiveFailures), MaxIntervalSeconds);
        _timer?.Change(TimeSpan.FromSeconds(delay), Timeout.InfiniteTimeSpan);
    }

    private static bool AreEqual(List<AppAnnouncement> a, List<AppAnnouncement> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Id != b[i].Id || a[i].Title != b[i].Title ||
                a[i].Content != b[i].Content || a[i].Type != b[i].Type ||
                a[i].ImageUrl != b[i].ImageUrl)
                return false;
        }
        return true;
    }

    private static async Task<List<AppAnnouncement>> FetchAsync()
    {
        try
        {
            string json = await Http.GetStringAsync(BackendApiRoutes.Announcements).ConfigureAwait(false);
            ApiResponse? resp = JsonSerializer.Deserialize<ApiResponse>(json);
            if (resp is { RetCode: 0, Data: { } data })
            {
                return data;
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            // Network errors (timeout, SSL, connection refused) are expected in
            // poor network conditions and should not pollute Sentry.
            if (ex is not (TaskCanceledException or HttpRequestException or OperationCanceledException))
            {
                SentrySdk.CaptureException(ex);
            }
        }

        return [];
    }

    internal sealed class AppAnnouncement
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "info";

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = "";

        [JsonPropertyName("link_url")]
        public string LinkUrl { get; set; } = "";

        [JsonPropertyName("link_text")]
        public string LinkText { get; set; } = "";

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = "";
    }

    private sealed class ApiResponse
    {
        [JsonPropertyName("retcode")]
        public int RetCode { get; set; }

        [JsonPropertyName("data")]
        public List<AppAnnouncement>? Data { get; set; }
    }
}
