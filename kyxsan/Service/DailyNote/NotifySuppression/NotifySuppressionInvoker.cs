//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;

namespace kyxsan.Service.DailyNote.NotifySuppression;

internal static class NotifySuppressionInvoker
{
    public static void Check(DailyNoteEntry entry, out List<DailyNoteNotifyInfo> infos)
    {
        infos = [];
        NotifySuppressionContext context = new(entry, infos);
        context.Invoke<ResinNotifySuppressionChecker>();
        context.Invoke<HomeCoinNotifySuppressionChecker>();
        context.Invoke<DailyTaskNotifySuppressionChecker>();
        context.Invoke<TransformerNotifySuppressionChecker>();
        context.Invoke<ExpeditionNotifySuppressionChecker>();
        context.Invoke<WeekActiveProgressNotifySuppressionChecker>();
    }

    extension(INotifySuppressionContext context)
    {
        [SuppressMessage("", "CA1859")]
        private void Invoke<T>()
            where T : INotifySuppressionChecker, new()
        {
            T checker = new();

            // Reach the notify threshold
            if (checker.ShouldNotify(context))
            {
                // If the suppression status is not set, we need to append notify info
                if (!checker.GetIsSuppressed(context))
                {
                    context.Add(checker.NotifyInfo(context));
                    checker.SetIsSuppressed(context, true);
                }
            }
            else
            {
                // Reset suppression status
                checker.SetIsSuppressed(context, false);
            }
        }
    }
}