//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace kyxsan.Core.Threading;

internal sealed class TaskContextWrapperForDispatcherQueue : ITaskContext
{
    public TaskContextWrapperForDispatcherQueue(DispatcherQueue dispatcherQueue)
    {
        DispatcherQueue = dispatcherQueue;
    }

    public DispatcherQueue DispatcherQueue { get; }

    public void BeginInvokeOnMainThread(Action action)
    {
        DispatcherQueue.TryEnqueue(() => action());
    }

    public void InvokeOnMainThread(Action action)
    {
        DispatcherQueue.Invoke(action);
    }

    public T InvokeOnMainThread<T>(Func<T> func)
    {
        return DispatcherQueue.Invoke(func);
    }

    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(DispatcherQueue);
    }

    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(DispatcherQueue);
    }
}