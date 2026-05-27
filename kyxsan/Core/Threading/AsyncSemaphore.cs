//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-5-asyncsemaphore/
[SuppressMessage("", "SH003")]
internal sealed class AsyncSemaphore
{
    private readonly Queue<TaskCompletionSource> waiters = [];
    private readonly int maxCount;
    private int currentCount;

    public AsyncSemaphore(int initialCount, int maxCount = int.MaxValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(initialCount);
        currentCount = initialCount;
        this.maxCount = maxCount;
    }

    public Task WaitAsync()
    {
        lock (waiters)
        {
            if (currentCount > 0)
            {
                --currentCount;
                return Task.CompletedTask;
            }

            TaskCompletionSource waiter = new();
            waiters.Enqueue(waiter);
            return waiter.Task;
        }
    }

    public bool TryWait()
    {
        lock (waiters)
        {
            if (currentCount > 0)
            {
                --currentCount;
                return true;
            }

            return false;
        }
    }

    public int Release()
    {
        TaskCompletionSource? toRelease = default;
        int currentCountForReturn;
        lock (waiters)
        {
            if (waiters.Count > 0)
            {
                toRelease = waiters.Dequeue();
            }
            else
            {
                currentCount = Math.Min(currentCount + 1, maxCount);
            }

            currentCountForReturn = currentCount;
        }

        toRelease?.SetResult();
        return currentCountForReturn;
    }
}