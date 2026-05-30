//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Web.Response;

namespace kyxsan.Web.Hoyolab.Hk4e.Common.Announcement;

internal sealed class AnnouncementWrapper : ListWrapper<AnnouncementListWrapper>, IJsonOnDeserialized
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("type_list")]
    public List<AnnouncementType> TypeList { get; set; } = default!;

    [JsonPropertyName("alert")]
    public bool Alert { get; set; }

    [JsonPropertyName("alert_id")]
    public int AlertId { get; set; }

    [JsonPropertyName("timezone")]
    public int TimeZone { get; set; }

    [JsonPropertyName("t")]
    public string TimeStamp { get; set; } = default!;

    /// <summary>
    /// 服务器时间与本地时间的差值（用于校准时间）
    /// </summary>
    [JsonIgnore]
    public TimeSpan ServerTimeOffset { get; private set; }

    public void OnDeserialized()
    {
        TimeSpan offset = TimeSpan.FromHours(TimeZone);

        // 计算服务器时间与本地时间的差值
        if (!string.IsNullOrEmpty(TimeStamp) && DateTime.TryParse(TimeStamp, out DateTime serverTime))
        {
            DateTimeOffset serverTimeWithOffset = new(serverTime, offset);
            ServerTimeOffset = serverTimeWithOffset - DateTimeOffset.Now;
        }

        foreach (AnnouncementListWrapper wrapper in List)
        {
            foreach (Announcement item in wrapper.List)
            {
                item.StartTime = UnsafeDateTimeOffset.AdjustOffsetOnly(item.StartTime, offset);
                item.EndTime = UnsafeDateTimeOffset.AdjustOffsetOnly(item.EndTime, offset);
                item.ServerTimeOffset = ServerTimeOffset;
            }
        }
    }
}