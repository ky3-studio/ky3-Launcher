//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Text.Json.Converter;

namespace kyxsan.Web.Hoyolab.Hk4e.Common.Announcement;

internal sealed class Announcement : AnnouncementContent, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 服务器时间与本地时间的差值（用于校准时间）
    /// </summary>
    [JsonIgnore]
    public TimeSpan ServerTimeOffset { get; set; }

    #region Binding

    public bool ShouldShowTimeDescription
    {
        get => StartTime != default && EndTime != default;
    }

    public string TimeDescription
    {
        get
        {
            // 使用服务器时间校准后的当前时间
            DateTimeOffset now = DateTimeOffset.UtcNow + ServerTimeOffset;

            if (StartTime > now)
            {
                return FormatCountdown(StartTime - now, true);
            }

            if (EndTime <= now)
            {
                return SH.WebAnnouncementTimeEnded;
            }

            return FormatCountdown(EndTime - now, false);
        }
    }

    public bool ShouldShowTimePercent
    {
        get => ShouldShowTimeDescription && TimePercent is > 0 and < 1;
    }

    public double TimePercent
    {
        get
        {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            TimeSpan current = currentTime - StartTime;
            TimeSpan total = EndTime - StartTime;
            return current / total;
        }
    }

    public string TimeFormatted
    {
        get => $"{StartTime.ToLocalTime():yyyy.MM.dd HH:mm} - {EndTime.ToLocalTime():yyyy.MM.dd HH:mm}";
    }
    #endregion

    public void NotifyTimePropertiesChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeDescription)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimePercent)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShouldShowTimePercent)));
    }

    private static string FormatCountdown(TimeSpan span, bool isStart)
    {
        int days = (int)span.TotalDays;
        int hours = span.Hours;
        int minutes = span.Minutes;
        int seconds = span.Seconds;

        string suffix = isStart ? SH.WebAnnouncementTimeSuffixBegin : SH.WebAnnouncementTimeSuffixEnd;

        return days > 0
            ? $"{days}\u5929 {hours:D2}:{minutes:D2}:{seconds:D2} {suffix}"
            : $"{hours:D2}:{minutes:D2}:{seconds:D2} {suffix}";
    }

    [JsonPropertyName("type_label")]
    public string TypeLabel { get; set; } = default!;

    [JsonPropertyName("tag_label")]
    public string TagLabel { get; set; } = default!;

    [JsonPropertyName("tag_icon")]
    public string TagIcon { get; set; } = default!;

    [JsonPropertyName("login_alert")]
    public int LoginAlert { get; set; }

    [JsonPropertyName("start_time")]
    [JsonConverter(typeof(SimpleDateTimeOffsetConverter))]
    public DateTimeOffset StartTime { get; set; }

    [JsonPropertyName("end_time")]
    [JsonConverter(typeof(SimpleDateTimeOffsetConverter))]
    public DateTimeOffset EndTime { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("remind")]
    public int Remind { get; set; }

    [JsonPropertyName("alert")]
    public int Alert { get; set; }

    [JsonPropertyName("tag_start_time")]
    public string TagStartTime { get; set; } = default!;

    [JsonPropertyName("tag_end_time")]
    public string TagEndTime { get; set; } = default!;

    [JsonPropertyName("remind_ver")]
    public int RemindVersion { get; set; }

    [JsonPropertyName("has_content")]
    public bool HasContent { get; set; }
}