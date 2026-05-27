//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-2-asyncautoresetevent/
[SuppressMessage("", "SH003")]
internal sealed class AsyncAutoResetEvent
{
    private readonly Queue<TaskCompletionSource> waits = [];
    private bool signaled;

    public AsyncAutoResetEvent(bool initialState)
    {
        signaled = initialState;
    }

    public Task WaitOneAsync()
    {
        lock (waits)
        {
            if (signaled)
            {
                signaled = false;
                return Task.CompletedTask;
            }

            TaskCompletionSource tcs = new();
            waits.Enqueue(tcs);
            return tcs.Task;
        }
    }

    public void Set()
    {
        TaskCompletionSource? toRelease = default;
        lock (waits)
        {
            if (waits.Count > 0)
            {
                toRelease = waits.Dequeue();
            }
            else if (!signaled)
            {
                signaled = true;
            }
        }

        toRelease?.SetResult();
    }
}