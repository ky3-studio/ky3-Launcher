//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Launcher.Core.Diagnostics;

internal readonly struct MeasureExecutionToken : IDisposable
{
    private readonly ValueStopwatch stopwatch;
    private readonly ILogger logger;
    private readonly string callerName;

    public MeasureExecutionToken(in ValueStopwatch stopwatch, ILogger logger, string callerName)
    {
        this.stopwatch = stopwatch;
        this.logger = logger;
        this.callerName = callerName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogExecutionTime(callerName, stopwatch.GetElapsedTime().TotalMilliseconds);
        }
    }
}


internal static partial class MeasureExecutionTokenLog
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "{Caller} took {Time} ms")]
    public static partial void LogExecutionTime(this ILogger logger, string caller, double time);
}
