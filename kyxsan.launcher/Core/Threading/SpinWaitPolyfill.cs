//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using System.Diagnostics;

namespace kyxsan.Core.Threading;

internal static class SpinWaitPolyfill
{
    public static unsafe void SpinWhile(delegate*<bool> condition)
    {
        SpinWait spinner = default;
        while (condition())
        {
            spinner.SpinOnce();
        }
    }

    public static unsafe void SpinWhile<T>(T state, delegate*<T, bool> condition)
    {
        SpinWait spinner = default;
        while (condition(state))
        {
            spinner.SpinOnce();
        }
    }

    public static void SpinUntil<T>(T state, [RequireStaticDelegate] Func<T, bool> condition)
    {
        SpinWait spinner = default;
        while (!condition(state))
        {
            spinner.SpinOnce();
        }
    }

    public static unsafe void SpinUntil<T>(ref readonly T state, delegate*<ref readonly T, bool> condition)
    {
        SpinWait spinner = default;
        while (!condition(in state))
        {
            spinner.SpinOnce();
        }
    }

    public static unsafe bool SpinUntil<T>(ref readonly T state, delegate*<ref readonly T, bool> condition, TimeSpan timeout)
    {
        long startTime = Stopwatch.GetTimestamp();

        SpinWait spinner = default;
        while (!condition(in state))
        {
            spinner.SpinOnce();

            if (timeout < Stopwatch.GetElapsedTime(startTime))
            {
                return false;
            }
        }

        return true;
    }
}