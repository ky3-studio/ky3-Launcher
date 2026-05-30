// kyxsan - App Announcement Service
// Fetches software announcements from the admin backend.

using kyxsan.Core.Setting;
using System.Net.Http;

namespace kyxsan.Service.RemoteConfig;

internal static class AppAnnouncementService
{
    private const string AnnouncementsUrl = "https://8.134.75.17:9000/api/announcements";
    internal static readonly HttpClient Http = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    }) { Timeout = TimeSpan.FromSeconds(5) };
    private static readonly object Lock = new();
    private static List<AppAnnouncement> _cached = [];
    private static Timer? _timer;
    private static bool _firstPoll = true;

    private static HashSet<int> _seenIds = LoadSeenIds();
    private static readonly HashSet<int> _notifiedThisSession = [];

    public static event Action<List<AppAnnouncement>>? Changed;

    public static List<AppAnnouncement> Current
    {
        get { lock (Lock) { return [.. _cached]; } }
    }

    public static bool ShouldNotify(List<AppAnnouncement> items)
    {
        lock (Lock)
        {
            bool hasNew = false;
            foreach (AppAnnouncement a in items)
            {
                if (!_seenIds.Contains(a.Id) && !_notifiedThisSession.Contains(a.Id))
                {
                    _notifiedThisSession.Add(a.Id);
                    hasNew = true;
                }
            }
            return hasNew;
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
        _timer = new Timer(async _ =>
        {
            List<AppAnnouncement> latest = await FetchAsync().ConfigureAwait(false);
            bool fire;
            lock (Lock)
            {
                bool changed = !AreEqual(_cached, latest);
                fire = _firstPoll || changed;
                if (fire) _cached = latest;
                _firstPoll = false;
            }
            if (fire) Changed?.Invoke(latest);
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
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
            string json = await Http.GetStringAsync(AnnouncementsUrl).ConfigureAwait(false);
            ApiResponse? resp = JsonSerializer.Deserialize<ApiResponse>(json);
            if (resp is { RetCode: 0, Data: { } data })
            {
                return data;
            }
        }
        catch
        {
            // Silently fail
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
