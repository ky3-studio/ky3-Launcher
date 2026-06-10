//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Service.DailyNote.NotifySuppression;

internal sealed class ResinNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool ShouldNotify(INotifySuppressionContext context)
    {
        bool result = context.DailyNote.CurrentResin >= context.Entry.ResinNotifyThreshold;
        context.Entry.ResinDotVisible = result;
        return result;
    }

    public bool GetIsSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.ResinNotifySuppressed;
    }

    public void SetIsSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.ResinNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierResin, $"{context.DailyNote.CurrentResin}", SH.FormatServiceDailyNoteNotifierResinCurrent(context.DailyNote.CurrentResin));
    }
}