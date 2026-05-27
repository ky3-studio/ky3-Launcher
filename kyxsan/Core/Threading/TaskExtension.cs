//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;

namespace kyxsan.Core.Threading;

internal static class TaskExtension
{
    extension(Task task)
    {
        public async void SafeForget()
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandling.KillProcessOnDbExceptionNoThrow(ex);
                ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
                SentrySdk.CaptureException(ex);
            }
        }
    }

    extension(ValueTask task)
    {
        public async void SafeForget()
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                ExceptionHandling.KillProcessOnDbExceptionNoThrow(ex);
                ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
                SentrySdk.CaptureException(ex);
            }
        }
    }
}