//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model.Entity.Abstraction;
using kyxsan.ViewModel.DailyNote;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kyxsan.Model.Entity;

[Table("daily_notes")]
internal sealed partial class DailyNoteEntry : ObservableObject, IAppDbEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;

    public string Uid { get; set; } = default!;

    public DailyNote? DailyNote { get; set; }

    public DateTimeOffset RefreshTime { get; set; }

    public int ResinNotifyThreshold { get; set; }

    public bool ResinNotifySuppressed { get; set; }

    public bool ResinDotVisible { get; set; }

    public int HomeCoinNotifyThreshold { get; set; }

    public bool HomeCoinNotifySuppressed { get; set; }

    public bool HomeCoinDotVisible { get; set; }

    public bool TransformerNotify { get; set; }

    public bool TransformerNotifySuppressed { get; set; }

    public bool TransformerDotVisible { get; set; }

    public bool DailyTaskNotify { get; set; }

    public bool DailyTaskNotifySuppressed { get; set; }

    public bool DailyTaskDotVisible { get; set; }

    public bool ExpeditionNotify { get; set; }

    public bool ExpeditionNotifySuppressed { get; set; }

    public bool ExpeditionDotVisible { get; set; }

    public bool WeekActiveProgressNotify { get; set; }

    public bool WeekActiveProgressNotifySuppressed { get; set; }

    public bool WeekActiveProgressDotVisible { get; set; }

    [NotMapped]
    public UserGameRole? UserGameRole { get; set; }

    [NotMapped]
    public DailyNoteArchonQuestView? ArchonQuestView { get; set; }

    [NotMapped]
    public string RefreshTimeFormatted
    {
        get
        {
            return RefreshTime == default
                ? SH.ModelEntityDailyNoteNotRefreshed
                : SH.FormatModelEntityDailyNoteRefreshTime(RefreshTime.ToLocalTime());
        }
    }

    public static DailyNoteEntry From(UserAndUid userAndUid)
    {
        return new()
        {
            UserId = userAndUid.User.InnerId,
            Uid = userAndUid.Uid.Value,
            ResinNotifyThreshold = 120,
            HomeCoinNotifyThreshold = 1800,
        };
    }

    public void Update(DailyNote? dailyNote)
    {
        DailyNote = dailyNote;
        OnPropertyChanged(nameof(DailyNote));

        RefreshTime = DateTimeOffset.UtcNow;
        OnPropertyChanged(nameof(RefreshTimeFormatted));
    }

    public void CopyTo(DailyNoteEntry other)
    {
        other.Update(DailyNote);

        other.ResinDotVisible = ResinDotVisible;
        other.OnPropertyChanged(nameof(ResinDotVisible));

        other.HomeCoinDotVisible = HomeCoinDotVisible;
        other.OnPropertyChanged(nameof(HomeCoinDotVisible));

        other.TransformerDotVisible = TransformerDotVisible;
        other.OnPropertyChanged(nameof(TransformerDotVisible));

        other.DailyTaskDotVisible = DailyTaskDotVisible;
        other.OnPropertyChanged(nameof(DailyTaskDotVisible));

        other.ExpeditionDotVisible = ExpeditionDotVisible;
        other.OnPropertyChanged(nameof(ExpeditionDotVisible));

        other.WeekActiveProgressDotVisible = WeekActiveProgressDotVisible;
        other.OnPropertyChanged(nameof(WeekActiveProgressDotVisible));
    }
}