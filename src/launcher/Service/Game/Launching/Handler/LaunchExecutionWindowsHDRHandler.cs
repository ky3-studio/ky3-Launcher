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

internal sealed class LaunchExecutionWindowsHDRHandler : AbstractLaunchExecutionHandler
{
    private bool hdrTurnedOnByHandler;

    public override ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (!context.LaunchOptions.IsWindowsHDREnabled.Value)
        {
            return ValueTask.CompletedTask;
        }

        bool hdrWasOn = Service.Game.Account.WindowsHDRControl.IsHDROn();
        if (!hdrWasOn)
        {
            Service.Game.Account.WindowsHDRControl.SetHDR(true);
            hdrTurnedOnByHandler = true;
        }

        return ValueTask.CompletedTask;
    }

    public override ValueTask AfterAsync(AfterLaunchExecutionContext context)
    {
        if (hdrTurnedOnByHandler)
        {
            Service.Game.Account.WindowsHDRControl.SetHDR(false);
            hdrTurnedOnByHandler = false;
        }

        return ValueTask.CompletedTask;
    }
}