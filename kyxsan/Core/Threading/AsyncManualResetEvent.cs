//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Threading;

using System.Threading; // 添加此 using 语句
using System.Threading.Tasks; // 添加此 using 语句

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-1-asyncmanualresetevent/
[SuppressMessage("", "SH003")]
internal sealed class AsyncManualResetEvent
{
    private volatile TaskCompletionSource tcs = new();

    public Task WaitAsync()
    {
        return tcs.Task;
    }

    [SuppressMessage("", "SH007")]
    public void Set()
    {
        TaskCompletionSource tcs = this.tcs;
        Task.Factory.StartNew(s => ((TaskCompletionSource)s!).TrySetResult(), tcs, System.Threading.CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
        tcs.Task.Wait();
    }

    public void Reset()
    {
        while (true)
        {
            TaskCompletionSource tcs = this.tcs;
            if (!tcs.Task.IsCompleted || Interlocked.CompareExchange(ref this.tcs, new(), tcs) == tcs)
            {
                return;
            }
        }
    }
}