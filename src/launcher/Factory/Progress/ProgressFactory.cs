//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;

namespace kyxsan.Factory.Progress;

[Service(ServiceLifetime.Transient, typeof(IProgressFactory))]
internal sealed partial class ProgressFactory : IProgressFactory
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial ProgressFactory(IServiceProvider serviceProvider);

    public IProgress<T> CreateForMainThread<T>(Action<T> handler)
    {
        if (taskContext is not ITaskContextUnsafe @unsafe)
        {
            throw kyxsanException.NotSupported();
        }

        return new DispatcherQueueProgress<T>(handler, @unsafe.DispatcherQueue);
    }

    public IProgress<T> CreateForMainThread<T, TState>(Action<T, TState> handler, TState state)
    {
        if (taskContext is not ITaskContextUnsafe @unsafe)
        {
            throw kyxsanException.NotSupported();
        }

        return new DispatcherQueueProgress<T, TState>(handler, state, @unsafe.DispatcherQueue);
    }
}