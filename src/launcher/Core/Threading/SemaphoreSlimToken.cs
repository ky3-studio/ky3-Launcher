//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

namespace Launcher.Core.Threading;

internal readonly struct SemaphoreSlimToken : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim;

    public SemaphoreSlimToken(SemaphoreSlim semaphoreSlim)
    {
        this.semaphoreSlim = semaphoreSlim;
    }

    public void Dispose()
    {
        try
        {
            semaphoreSlim.Release();
        }
        catch (ObjectDisposedException)
        {
            // ViewModel disposed while async operation was still holding the semaphore
        }
    }
}