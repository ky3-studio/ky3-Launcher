using kyxsan.Core.Threading;
using Xunit;

namespace ky3launcher.Tests.Threading;

public class AsyncLockTests
{
    [Fact]
    public async Task LockAsync_SingleAcquire_Succeeds()
    {
        AsyncLock asyncLock = new();

        using (AsyncLock.Releaser releaser = await asyncLock.LockAsync())
        {
            Assert.True(true);
        }
    }

    [Fact]
    public async Task LockAsync_DoubleAcquire_SecondWaitsUntilFirstReleased()
    {
        AsyncLock asyncLock = new();
        bool secondEntered = false;

        using (AsyncLock.Releaser first = await asyncLock.LockAsync())
        {
            Task secondTask = Task.Run(async () =>
            {
                using (AsyncLock.Releaser second = await asyncLock.LockAsync())
                {
                    secondEntered = true;
                }
            });

            await Task.Delay(100);
            Assert.False(secondEntered);
        }

        await Task.Delay(200);
        Assert.True(secondEntered);
    }

    [Fact]
    public void TryLock_WhenAvailable_ReturnsTrue()
    {
        AsyncLock asyncLock = new();

        bool acquired = asyncLock.TryLock(out AsyncLock.Releaser releaser);

        Assert.True(acquired);
        releaser.Dispose();
    }

    [Fact]
    public async Task TryLock_WhenHeld_ReturnsFalse()
    {
        AsyncLock asyncLock = new();

        using (AsyncLock.Releaser held = await asyncLock.LockAsync())
        {
            bool acquired = asyncLock.TryLock(out AsyncLock.Releaser discard);
            Assert.False(acquired);
        }
    }

    [Fact]
    public async Task LockAsync_ConcurrentAccess_EnsuresMutualExclusion()
    {
        AsyncLock asyncLock = new();
        int counter = 0;
        int maxConcurrent = 0;
        int currentConcurrent = 0;

        Task[] tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(async () =>
        {
            using (AsyncLock.Releaser releaser = await asyncLock.LockAsync())
            {
                int current = Interlocked.Increment(ref currentConcurrent);
                if (current > maxConcurrent)
                {
                    Interlocked.Exchange(ref maxConcurrent, current);
                }

                Interlocked.Increment(ref counter);
                await Task.Delay(10);

                Interlocked.Decrement(ref currentConcurrent);
            }
        })).ToArray();

        await Task.WhenAll(tasks);

        Assert.Equal(10, counter);
        Assert.Equal(1, maxConcurrent);
    }
}
