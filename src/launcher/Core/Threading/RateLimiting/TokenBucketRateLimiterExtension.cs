//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Threading.RateLimiting;

namespace kyxsan.Core.Threading.RateLimiting;

internal static class TokenBucketRateLimiterExtension
{
    extension(TokenBucketRateLimiter rateLimiter)
    {
        // IMPORTANT: acquired can be none 0 values if false is returned
        public bool TryAcquire(int permits, out int acquired, out TimeSpan retryAfter)
        {
            lock (PrivateGetLock(rateLimiter))
            {
                acquired = (int)Math.Min(permits, PrivateGetTokenCount(rateLimiter));

                using (RateLimitLease lease = rateLimiter.AttemptAcquire(acquired))
                {
                    lease.TryGetMetadata(MetadataName.RetryAfter, out retryAfter);
                    return lease.IsAcquired;
                }
            }
        }

        public void Replenish(int permits)
        {
            if (permits <= 0)
            {
                return;
            }

            lock (PrivateGetLock(rateLimiter))
            {
                ref double tokenCount = ref PrivateGetTokenCount(rateLimiter);
                tokenCount = Math.Min(PrivateGetOptions(rateLimiter).TokenLimit, tokenCount + permits);
            }
        }
    }

    // private object Lock => _queue;
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Lock")]
    private static extern object PrivateGetLock(TokenBucketRateLimiter rateLimiter);

    // private double _tokenCount;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_tokenCount")]
    private static extern ref double PrivateGetTokenCount(TokenBucketRateLimiter rateLimiter);

    // private readonly TokenBucketRateLimiterOptions _options;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_options")]
    private static extern ref TokenBucketRateLimiterOptions PrivateGetOptions(TokenBucketRateLimiter rateLimiter);
}