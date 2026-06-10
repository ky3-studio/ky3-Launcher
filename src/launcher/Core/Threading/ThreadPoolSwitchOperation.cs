//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using kyxsan.Core.Threading.Abstraction;

namespace kyxsan.Core.Threading;

internal readonly struct ThreadPoolSwitchOperation : IAwaitable<ThreadPoolSwitchOperation>, ICriticalAwaiter
{
    private static readonly Action<Action> WaitCallbackRunAction = RunAction;
    private readonly DispatcherQueue dispatcherQueue;

    public ThreadPoolSwitchOperation(DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
    }

    public bool IsCompleted
    {
        // Only yields when we are on the DispatcherQueue thread.
        get => !dispatcherQueue.HasThreadAccess;
    }

    public ThreadPoolSwitchOperation GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
    }

    public void OnCompleted(Action continuation)
    {
        ThreadPool.QueueUserWorkItem(WaitCallbackRunAction, continuation, false);
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        ThreadPool.UnsafeQueueUserWorkItem(WaitCallbackRunAction, continuation, false);
    }

    private static void RunAction(Action state)
    {
        state();
    }
}