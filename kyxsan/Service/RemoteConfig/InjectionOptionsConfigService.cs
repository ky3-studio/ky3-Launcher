// kyxsan - Remote Injection Options Config Service
// Fetches injection option visibility from the admin backend.
// Supports periodic refresh so admin changes take effect in real-time.

using System.Net.Http;

namespace kyxsan.Service.RemoteConfig;

internal static class InjectionOptionsConfigService
{
    private const string ConfigUrl = "https://8.134.75.17:9000/api/config";
    private const int TotalOptionCount = 16;
    private static readonly HttpClient Http = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    }) { Timeout = TimeSpan.FromSeconds(5) };
    private static readonly object Lock = new();

    private static HashSet<string> _disabledKeys = [];
    private static Timer? _refreshTimer;

    public static HashSet<string> Current
    {
        get { lock (Lock) { return [.. _disabledKeys]; } }
    }

    /// <summary>
    /// Fired when the disabled keys set changes. Listeners apply visibility on UI thread.
    /// </summary>
    public static event Action<HashSet<string>>? Changed;

    /// <summary>
    /// Start background polling (every 10s). Safe to call multiple times.
    /// </summary>
    public static void StartPolling()
    {
        if (_refreshTimer is not null) return;
        _refreshTimer = new Timer(async _ =>
        {
            HashSet<string> latest = await FetchDisabledKeysAsync().ConfigureAwait(false);
            bool changed;
            lock (Lock)
            {
                changed = !_disabledKeys.SetEquals(latest);
                if (changed) _disabledKeys = latest;
            }

            if (changed)
            {
                Changed?.Invoke(latest);
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    private static async Task<HashSet<string>> FetchDisabledKeysAsync()
    {
        HashSet<string> disabled = [];
        try
        {
            string json = await Http.GetStringAsync(ConfigUrl).ConfigureAwait(false);
            ConfigResponse? resp = JsonSerializer.Deserialize<ConfigResponse>(json);
            if (resp?.Data?.InjectionOptions is { } options)
            {
                foreach (InjectionOption opt in options)
                {
                    if (!opt.Enabled)
                    {
                        disabled.Add(opt.Key);
                    }
                }
            }

            // Safety: if ALL options are disabled, treat as misconfiguration — show everything
            if (disabled.Count >= TotalOptionCount)
            {
                disabled.Clear();
            }
        }
        catch
        {
            // Fail-open: if server is unreachable, show all options
        }

        return disabled;
    }

    private sealed class ConfigResponse
    {
        [JsonPropertyName("retcode")]
        public int RetCode { get; set; }

        [JsonPropertyName("data")]
        public ConfigData? Data { get; set; }
    }

    private sealed class ConfigData
    {
        [JsonPropertyName("injection_options")]
        public List<InjectionOption>? InjectionOptions { get; set; }
    }

    private sealed class InjectionOption
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = "";

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;
    }
}
