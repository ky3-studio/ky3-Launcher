//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.Launching.Context;

namespace kyxsan.Service.Game.Launching.Handler;

internal abstract class AbstractLaunchExecutionHandler : ILaunchExecutionHandler
{
    public virtual ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask AfterAsync(AfterLaunchExecutionContext context)
    {
        return ValueTask.CompletedTask;
    }
}