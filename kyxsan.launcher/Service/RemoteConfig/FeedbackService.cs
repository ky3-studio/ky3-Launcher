// kyxsan - Feedback Service
// Handles feedback submission and reply polling from the admin backend.

using kyxsan.Core;
using System.IO;
using System.Net.Http;
using System.Text;

namespace kyxsan.Service.RemoteConfig;

internal static class FeedbackService
{
    private const string BaseUrl = "https://8.134.75.17:9000/api";
    private static readonly HttpClient Http = new(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        }) { Timeout = TimeSpan.FromSeconds(15) };

    /// <summary>Upload a feedback image (no auth required). Returns the hosted URL or null on failure.</summary>
    public static async Task<string?> UploadImageAsync(string filePath)
    {
        try
        {
            using MultipartFormDataContent form = [];
            byte[] bytes = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
            ByteArrayContent fileContent = new(bytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                Path.GetExtension(filePath).ToLowerInvariant() switch
                {
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".bmp" => "image/bmp",
                    _ => "image/jpeg",
                });
            form.Add(fileContent, "file", Path.GetFileName(filePath));
            HttpResponseMessage resp = await Http.PostAsync($"{BaseUrl}/feedback-image", form).ConfigureAwait(false);
            string json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.GetProperty("retcode").GetInt32() == 0)
                return doc.RootElement.GetProperty("data").GetProperty("url").GetString();
        }
        catch
        {
            // Silently fail
        }

        return null;
    }

    /// <summary>Submit feedback with optional image URLs.</summary>
    public static async Task<bool> SubmitFeedbackAsync(string content, string contact, string version, List<string> imageUrls)
    {
        try
        {
            string json = JsonSerializer.Serialize(new
            {
                device_id = kyxsanRuntime.DeviceId,
                content,
                contact,
                version,
                images = imageUrls,
            });
            StringContent httpContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage resp = await Http.PostAsync($"{BaseUrl}/feedback", httpContent).ConfigureAwait(false);
            string respJson = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            using JsonDocument doc = JsonDocument.Parse(respJson);
            return doc.RootElement.GetProperty("retcode").GetInt32() == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Get unread admin replies for this device.</summary>
    public static async Task<List<FeedbackReply>> GetUnreadRepliesAsync()
    {
        try
        {
            string deviceId = Uri.EscapeDataString(kyxsanRuntime.DeviceId);
            string json = await Http.GetStringAsync($"{BaseUrl}/feedback/replies?device_id={deviceId}").ConfigureAwait(false);
            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.GetProperty("retcode").GetInt32() == 0 &&
                doc.RootElement.TryGetProperty("data", out JsonElement dataEl))
            {
                return JsonSerializer.Deserialize<List<FeedbackReply>>(dataEl.GetRawText()) ?? [];
            }
        }
        catch
        {
            // Silently fail
        }

        return [];
    }

    /// <summary>Mark all replies for this device as read.</summary>
    public static async Task MarkRepliesReadAsync()
    {
        try
        {
            object body = new { device_id = kyxsanRuntime.DeviceId };
            string json = JsonSerializer.Serialize(body);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            await Http.PostAsync($"{BaseUrl}/feedback/reply-read", content).ConfigureAwait(false);
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>Report client presence to the admin backend (heartbeat). Throws on failure.</summary>
    public static async Task HeartbeatAsync()
    {
        string json = JsonSerializer.Serialize(new
        {
            device_id = kyxsanRuntime.DeviceId,
            version = kyxsanRuntime.Version.ToString(),
            os_version = System.Environment.OSVersion.VersionString,
        });
        StringContent content = new(json, Encoding.UTF8, "application/json");
        await Http.PostAsync($"{BaseUrl}/heartbeat", content).ConfigureAwait(false);
    }
}

internal sealed class FeedbackReply
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("reply")]
    public string Reply { get; set; } = "";

    [JsonPropertyName("replied_at")]
    public string RepliedAt { get; set; } = "";
}
