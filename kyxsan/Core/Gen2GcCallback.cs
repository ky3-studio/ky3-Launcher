//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace kyxsan.Core;

// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Gen2GcCallback.cs
internal sealed class Gen2GcCallback : CriticalFinalizerObject
{
    private readonly Func<bool>? callback0;
    private readonly Func<object, bool>? callback1;
    private WeakGCHandle<object> weakTargetObj;

    private Gen2GcCallback(Func<bool> callback)
    {
        callback0 = callback;
    }

    private Gen2GcCallback(Func<object, bool> callback, object targetObj)
    {
        callback1 = callback;
        weakTargetObj = new(targetObj);
    }

    ~Gen2GcCallback()
    {
        if (weakTargetObj.IsAllocated)
        {
            if (!weakTargetObj.TryGetTarget(out object? targetObj))
            {
                weakTargetObj.Dispose();
                return;
            }

            try
            {
                Debug.Assert(callback1 is not null);
                if (!callback1(targetObj))
                {
                    weakTargetObj.Dispose();
                    return;
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }
        else
        {
            try
            {
                Debug.Assert(callback0 is not null);
                if (!callback0())
                {
                    return;
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }

        GC.ReRegisterForFinalize(this);
    }

    /// <summary>
    /// Schedule 'callback' to be called in the next GC.  If the callback returns true it is
    /// rescheduled for the next Gen 2 GC. Otherwise, the callbacks stop.
    /// </summary>
    /// <param name="callback">callback</param>
    public static void Register(Func<bool> callback)
    {
        _ = new Gen2GcCallback(callback);
    }

    /// <summary>
    /// Schedule 'callback' to be called in the next GC.  If the callback returns true it is
    /// rescheduled for the next Gen 2 GC. Otherwise, the callbacks stop.
    ///
    /// NOTE: This callback will be kept alive until either the callback function returns false,
    /// or the target object dies.
    /// </summary>
    /// <param name="callback">callback</param>
    /// <param name="targetObj">target</param>
    public static void Register(Func<object, bool> callback, object targetObj)
    {
        // Create an unreachable object that remembers the callback function and target object.
        _ = new Gen2GcCallback(callback, targetObj);
    }
}