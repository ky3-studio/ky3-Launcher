//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Diagnostics;
using kyxsan.Service.Game.Launching.Context;
using kyxsan.Service.Game.Launching.Handler;

namespace kyxsan.Service.Game.Launching.Invoker;

internal sealed class ResumeLaunchExecutionInvoker : AbstractLaunchExecutionInvoker
{
    public ResumeLaunchExecutionInvoker()
    {
        Handlers =
        [
            new LaunchExecutionGameLifeCycleHandler(resume: true)
        ];
    }

    protected override IProcess? CreateProcess(BeforeLaunchExecutionContext context)
    {
        context.TryGetOption(LaunchExecutionOptionsKey.RunningProcess, out IProcess? process);
        return process;
    }
}