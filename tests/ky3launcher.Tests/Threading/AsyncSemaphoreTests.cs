using kyxsan.Core.Threading;
using Xunit;

namespace ky3launcher.Tests.Threading;

public class AsyncSemaphoreTests
{
    [Fact]
    public async Task WaitAsync_WithAvailableCount_CompletesImmediately()
    {
        AsyncSemaphore semaphore = new(2);

        Task wait1 = semaphore.WaitAsync();
        Task wait2 = semaphore.WaitAsync();

        Assert.True(wait1.IsCompleted);
        Assert.True(wait2.IsCompleted);
    }

    [Fact]
    public async Task WaitAsync_ExceedsCount_Blocks()
    {
        AsyncSemaphore semaphore = new(1);

        Task wait1 = semaphore.WaitAsync();
        Task wait2 = semaphore.WaitAsync();

        Assert.True(wait1.IsCompleted);
        Assert.False(wait2.IsCompleted);

        semaphore.Release();
        await wait2;
        Assert.True(wait2.IsCompleted);
    }

    [Fact]
    public void TryWait_WhenAvailable_ReturnsTrue()
    {
        AsyncSemaphore semaphore = new(1);

        bool result = semaphore.TryWait();

        Assert.True(result);
    }

    [Fact]
    public void TryWait_WhenExhausted_ReturnsFalse()
    {
        AsyncSemaphore semaphore = new(1);

        semaphore.TryWait();
        bool result = semaphore.TryWait();

        Assert.False(result);
    }

    [Fact]
    public void Release_RestoresCount()
    {
        AsyncSemaphore semaphore = new(1);

        semaphore.TryWait();
        Assert.False(semaphore.TryWait());

        semaphore.Release();
        Assert.True(semaphore.TryWait());
    }

    [Fact]
    public void Release_RespectsMaxCount()
    {
        AsyncSemaphore semaphore = new(1, maxCount: 2);

        semaphore.Release();
        semaphore.Release();

        Assert.True(semaphore.TryWait());
        Assert.True(semaphore.TryWait());
        Assert.False(semaphore.TryWait());
    }

    [Fact]
    public async Task WaitAsync_FIFO_Order()
    {
        AsyncSemaphore semaphore = new(0);
        List<int> order = [];

        Task t1 = Task.Run(async () => { await semaphore.WaitAsync(); order.Add(1); });
        await Task.Delay(50);
        Task t2 = Task.Run(async () => { await semaphore.WaitAsync(); order.Add(2); });
        await Task.Delay(50);

        semaphore.Release();
        await Task.Delay(50);
        semaphore.Release();
        await Task.Delay(50);

        await Task.WhenAll(t1, t2);

        Assert.Equal(1, order[0]);
        Assert.Equal(2, order[1]);
    }
}
