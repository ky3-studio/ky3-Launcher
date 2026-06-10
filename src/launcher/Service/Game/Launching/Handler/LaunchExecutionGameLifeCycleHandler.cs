//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Service.Game.Launching.Context;

namespace kyxsan.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameLifeCycleHandler : AbstractLaunchExecutionHandler
{
    private readonly bool resume;

    public LaunchExecutionGameLifeCycleHandler(bool resume)
    {
        this.resume = resume;
    }

    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (resume)
        {
            if (await GameLifeCycle.TryGetRunningGameProcessAsync(context.TaskContext).ConfigureAwait(false) is (true, { } gameProcess))
            {
                context.SetOption(LaunchExecutionOptionsKey.RunningProcess, gameProcess);
            }
        }
        else
        {
            if (await GameLifeCycle.IsGameRunningAsync(context.TaskContext).ConfigureAwait(false))
            {
                kyxsanException.Throw(SH.ServiceGameLaunchExecutionGameIsRunning);
            }
        }
    }
}