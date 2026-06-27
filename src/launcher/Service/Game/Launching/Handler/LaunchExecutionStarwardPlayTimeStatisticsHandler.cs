//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.Diagnostics;
using Launcher.Service.Game.Launching.Context;
using Windows.System;

namespace Launcher.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionStarwardPlayTimeStatisticsHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (context.Process.IsRunning && context.LaunchOptions.UsingStarwardPlayTimeStatistics.Value)
        {
            await LaunchStarwardForPlayTimeStatisticsAsync(context).ConfigureAwait(false);
        }
    }

    private static async ValueTask LaunchStarwardForPlayTimeStatisticsAsync(LaunchExecutionContext context)
    {
        string gameBiz = context.IsOversea ? "hk4e_global" : "hk4e_cn";
        Uri starwardPlayTimeUri = $"starward://playtime/{gameBiz}".ToUri();
        if (await Windows.System.Launcher.QueryUriSupportAsync(starwardPlayTimeUri, LaunchQuerySupportType.Uri) is LaunchQuerySupportStatus.Available)
        {
            await Windows.System.Launcher.LaunchUriAsync(starwardPlayTimeUri);
        }
    }
}