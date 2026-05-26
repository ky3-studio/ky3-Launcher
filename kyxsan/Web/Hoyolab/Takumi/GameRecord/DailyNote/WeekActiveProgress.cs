//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class WeekActiveProgress
{
    [JsonPropertyName("progress_current")]
    public int ProgressCurrent { get; set; }

    [JsonPropertyName("progress_total")]
    public int ProgressTotal { get; set; }

    [JsonPropertyName("period_progress_current")]
    public int PeriodProgressCurrent { get; set; }

    [JsonPropertyName("period_progress_total")]
    public int PeriodProgressTotal { get; set; }

    [JsonPropertyName("unlock")]
    public bool Unlock { get; set; }

    [JsonPropertyName("progress_current_arr")]
    public List<int> ProgressCurrentArray { get; set; } = default!;

    [JsonPropertyName("is_active_period")]
    public bool IsActivePeriod { get; set; }

    [JsonPropertyName("current_weekday")]
    public int CurrentWeekday { get; set; }

    [JsonIgnore]
    public string FormattedProgress
    {
        get => $"{ProgressCurrent}/{ProgressTotal}";
    }

    [JsonIgnore]
    public string FormattedPeriodProgress
    {
        get => $"{PeriodProgressCurrent}/{PeriodProgressTotal}";
    }

    [JsonIgnore]
    public bool IsAllProgressFinished
    {
        get => Unlock && ProgressTotal > 0 && ProgressCurrent >= ProgressTotal;
    }

    [JsonIgnore]
    public string ProgressDescription
    {
        get
        {
            if (!Unlock)
            {
                return SH.WebDailyNoteWeekActiveProgressLocked;
            }

            return IsAllProgressFinished
                ? SH.WebDailyNoteWeekActiveProgressFinished
                : SH.FormatWebDailyNoteWeekActiveProgressPeriod(PeriodProgressCurrent, PeriodProgressTotal);
        }
    }
}
