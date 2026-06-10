//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Frozen;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class Expedition
{
    private const string Bennett = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Bennett.png";
    private const string Chongyun = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Chongyun.png";
    private const string Fischl = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Fischl.png";
    private const string Keqing = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Keqing.png";
    private const string Sara = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Sara.png";

    private static readonly FrozenSet<string> ShortExpeditionTimeAvatars = [Bennett, Chongyun, Fischl, Keqing, Sara];

    [JsonPropertyName("avatar_side_icon")]
    public Uri AvatarSideIcon { get; set; } = default!;

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ExpeditionStatus Status { get; set; }

    [JsonPropertyName("remained_time")]
    public int RemainedTime { get; set; }

    [JsonIgnore]
    public int PassedTime
    {
        get
        {
            // 当派遣非20小时的任务时无法兼容
            int hours = ShortExpeditionTimeAvatars.Contains(AvatarSideIcon.ToString()) ? 15 : 20;
            return (hours * 60 * 60) - RemainedTime;
        }
    }

    [JsonIgnore]
    public int TotalTime
    {
        get
        {
            // 当派遣非20小时的任务时无法兼容
            int hours = ShortExpeditionTimeAvatars.Contains(AvatarSideIcon.ToString()) ? 15 : 20;
            return hours * 60 * 60;
        }
    }

    [JsonIgnore]
    public string RemainedTimeFormatted
    {
        get
        {
            if (Status == ExpeditionStatus.Finished)
            {
                return SH.ServiceDailyNoteNotifierExpeditionAdaptiveHint;
            }

            TimeSpan ts = TimeSpan.FromSeconds(RemainedTime);
            return SH.FormatWebDailyNoteExpeditionRemainTime(ts.Hours, ts.Minutes);
        }
    }
}