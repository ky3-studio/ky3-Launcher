//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

namespace kyxsan.Service.DailyNote.NotifySuppression;

internal sealed class ExpeditionNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool ShouldNotify(INotifySuppressionContext context)
    {
        bool result = context.Entry.ExpeditionNotify && context.DailyNote.Expeditions.All(e => e.Status == ExpeditionStatus.Finished);
        context.Entry.ExpeditionDotVisible = result;
        return result;
    }

    public bool GetIsSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.ExpeditionNotifySuppressed;
    }

    public void SetIsSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.ExpeditionNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierExpedition, SH.ServiceDailyNoteNotifierExpeditionAdaptiveHint, SH.ServiceDailyNoteNotifierExpeditionHint);
    }
}