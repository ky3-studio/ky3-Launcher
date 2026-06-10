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

internal static class SemaphoreSlimExtension
{
    extension(SemaphoreSlim semaphoreSlim)
    {
        public async ValueTask<SemaphoreSlimToken> EnterAsync(CancellationToken token = default)
        {
            try
            {
                await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            }
            catch (ObjectDisposedException ex)
            {
                kyxsanException.OperationCanceled(SH.CoreThreadingSemaphoreSlimDisposed, ex);
            }

            return new(semaphoreSlim);
        }

        public SemaphoreSlimToken Enter()
        {
            try
            {
                semaphoreSlim.Wait();
            }
            catch (ObjectDisposedException ex)
            {
                kyxsanException.OperationCanceled(SH.CoreThreadingSemaphoreSlimDisposed, ex);
            }

            return new(semaphoreSlim);
        }
    }
}