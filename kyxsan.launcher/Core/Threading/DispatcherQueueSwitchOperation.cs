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

internal readonly struct DispatcherQueueSwitchOperation : IAwaitable<DispatcherQueueSwitchOperation>, ICriticalAwaiter
{
    private readonly DispatcherQueue dispatcherQueue;

    public DispatcherQueueSwitchOperation(DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
    }

    public bool IsCompleted
    {
        // Only yields when we are not on the DispatcherQueue thread.
        get => dispatcherQueue.HasThreadAccess;
    }

    public DispatcherQueueSwitchOperation GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
    }

    public void OnCompleted(Action continuation)
    {
        dispatcherQueue.TryEnqueue(new(continuation));
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        using (ExecutionContext.SuppressFlow())
        {
            dispatcherQueue.TryEnqueue(new(continuation));
        }
    }
}