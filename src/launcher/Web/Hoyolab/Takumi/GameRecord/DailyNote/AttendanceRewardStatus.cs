//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

namespace Launcher.Web.Hoyolab.Takumi.GameRecord.DailyNote;

[ExtendedEnum]
internal enum AttendanceRewardStatus
{
    [JsonStringEnumMemberName("AttendanceRewardStatusInvalid")]
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusInvalid))]
    Invalid,

    [JsonStringEnumMemberName("AttendanceRewardStatusTakenAward")]
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusTakenAward))]
    TakenAward,

    [JsonStringEnumMemberName("AttendanceRewardStatusWaitTaken")]
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusWaitTaken))]
    WaitTaken,

    [JsonStringEnumMemberName("AttendanceRewardStatusUnfinished")]
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusUnfinished))]
    Unfinished,

    [JsonStringEnumMemberName("AttendanceRewardStatusFinishedNonReward")]
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusFinishedNonReward))]
    FinishedNonReward,

    [JsonStringEnumMemberName("AttendanceRewardStatusForbid")]
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusForbid))]
    Forbid,
}
