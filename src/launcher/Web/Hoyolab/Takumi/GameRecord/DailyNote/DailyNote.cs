//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class DailyNote : DailyNoteCommon, IJsonOnDeserialized
{
    #region Binding
    [JsonIgnore]
    public string FormattedResin
    {
        get => $"{CurrentResin}/{MaxResin}";
    }

    [JsonIgnore]
    public string ResinRecoveryTargetTime
    {
        get
        {
            if (ResinRecoveryTime == 0)
            {
                return SH.WebDailyNoteResinRecoveryCompleted;
            }

            DateTimeOffset reach = DeserializeTime.AddSeconds(ResinRecoveryTime);
            return SH.FormatWebDailyNoteResinRecoveryTargetTime(reach);
        }
    }

    [JsonIgnore]
    public string FormattedTask
    {
        get => $"{FinishedTaskNum}/{TotalTaskNum}";
    }

    [JsonIgnore]
    public string ExtraTaskRewardDescription
    {
        get
        {
            return IsExtraTaskRewardReceived
                ? SH.WebDailyNoteExtraTaskRewardReceived
                : FinishedTaskNum == TotalTaskNum
                    ? SH.WebDailyNoteExtraTaskRewardNotTaken
                    : SH.WebDailyNoteExtraTaskRewardNotAllowed;
        }
    }

    [JsonIgnore]
    public int ResinDiscountUsedNum
    {
        get => ResinDiscountNumLimit - RemainResinDiscountNum;
    }

    [JsonIgnore]
    public string FormattedResinDiscount
    {
        get => $"{ResinDiscountUsedNum}/{ResinDiscountNumLimit}";
    }

    [JsonIgnore]
    public string FormattedHomeCoin
    {
        get => MaxHomeCoin == 0 ? SH.WebDailyNoteHomeLocked : $"{CurrentHomeCoin}/{MaxHomeCoin}";
    }

    [JsonIgnore]
    public string FormattedHomeCoinRecoveryTargetTime
    {
        get
        {
            if (HomeCoinRecoveryTime == 0)
            {
                return SH.WebDailyNoteHomeCoinRecoveryCompleted;
            }

            DateTimeOffset reach = DeserializeTime.AddSeconds(HomeCoinRecoveryTime);
            return SH.FormatWebDailyNoteHomeCoinRecoveryTargetTime(reach);
        }
    }

    [JsonIgnore]
    public bool IsArchonQuestFinished
    {
        get => ArchonQuestProgress.List.Count == 0;
    }
    #endregion

    [JsonPropertyName("remain_resin_discount_num")]
    public int RemainResinDiscountNum { get; init; }

    [JsonPropertyName("resin_discount_num_limit")]
    public int ResinDiscountNumLimit { get; init; }

    [JsonPropertyName("home_coin_recovery_time")]
    public int HomeCoinRecoveryTime { get; init; }

    [JsonPropertyName("calendar_url")]
    public string CalendarUrl { get; init; } = default!;

    [JsonPropertyName("transformer")]
    public Transformer Transformer { get; init; } = default!;

    [JsonPropertyName("daily_task")]
    public DailyTask DailyTask { get; init; } = default!;

    [JsonPropertyName("archon_quest_progress")]
    public ArchonQuestProgress ArchonQuestProgress { get; init; } = default!;

    [JsonPropertyName("week_active_progress")]
    public WeekActiveProgress WeekActiveProgress { get; init; } = default!;

    private DateTimeOffset DeserializeTime { get; set; }

    public void OnDeserialized()
    {
        DeserializeTime = DateTimeOffset.Now;
    }

    internal static DailyNote FromWidget(WidgetDailyNote widget)
    {
        DailyNote note = new()
        {
            CurrentExpeditionNum = widget.CurrentExpeditionNum,
            CurrentHomeCoin = widget.CurrentHomeCoin,
            CurrentResin = widget.CurrentResin,
            Expeditions = widget.Expeditions,
            FinishedTaskNum = widget.FinishedTaskNum,
            IsExtraTaskRewardReceived = widget.IsExtraTaskRewardReceived,
            MaxExpeditionNum = widget.MaxExpeditionNum,
            MaxHomeCoin = widget.MaxHomeCoin,
            MaxResin = widget.MaxResin,
            ResinRecoveryTime = widget.ResinRecoveryTime,
            TotalTaskNum = widget.TotalTaskNum,
            ArchonQuestProgress = new ArchonQuestProgress { List = [], IsFinishAllMainline = true },
            Transformer = new Transformer { Obtained = false },
            DailyTask = new DailyTask
            {
                TotalNum = widget.TotalTaskNum,
                FinishedNum = widget.FinishedTaskNum,
                IsExtraTaskRewardReceived = widget.IsExtraTaskRewardReceived,
                TaskRewards = [],
                AttendanceRewards = [],
            },
            WeekActiveProgress = new WeekActiveProgress { ProgressCurrentArray = [] },
            CalendarUrl = string.Empty,
        };

        note.OnDeserialized();
        return note;
    }
}