//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace kyxsan.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-4-asyncbarrier/
[SuppressMessage("", "SH003")]
internal sealed class AsyncBarrier
{
    private readonly int participantCount;
    private int remainingParticipants;
    private ConcurrentStack<TaskCompletionSource> waiters = [];

    public AsyncBarrier(int participantCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(participantCount);
        remainingParticipants = this.participantCount = participantCount;
    }

    public Task SignalAndWaitAsync()
    {
        TaskCompletionSource tcs = new();
        waiters.Push(tcs);
        if (Interlocked.Decrement(ref remainingParticipants) == 0)
        {
            remainingParticipants = participantCount;
            ConcurrentStack<TaskCompletionSource> waiters = this.waiters;
            this.waiters = [];
            Parallel.ForEach(waiters, w => w.SetResult());
        }

        return tcs.Task;
    }
}