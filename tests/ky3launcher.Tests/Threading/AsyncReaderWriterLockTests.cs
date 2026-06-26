using kyxsan.Core.Threading;
using Xunit;

namespace ky3launcher.Tests.Threading;

public class AsyncReaderWriterLockTests
{
    [Fact]
    public async Task ReaderLock_MultipleReaders_AllEnterConcurrently()
    {
        AsyncReaderWriterLock rwLock = new();
        int concurrentReaders = 0;
        int maxConcurrentReaders = 0;

        Task[] tasks = Enumerable.Range(0, 5).Select(i => Task.Run(async () =>
        {
            using (AsyncReaderWriterLock.Releaser r = await rwLock.ReaderLockAsync($"reader{i}"))
            {
                int current = Interlocked.Increment(ref concurrentReaders);
                if (current > maxConcurrentReaders)
                {
                    Interlocked.Exchange(ref maxConcurrentReaders, current);
                }

                await Task.Delay(50);
                Interlocked.Decrement(ref concurrentReaders);
            }
        })).ToArray();

        await Task.WhenAll(tasks);

        Assert.True(maxConcurrentReaders > 1);
    }

    [Fact]
    public async Task WriterLock_ExclusiveAccess()
    {
        AsyncReaderWriterLock rwLock = new();

        using (AsyncReaderWriterLock.Releaser w = await rwLock.WriterLockAsync("writer1"))
        {
            bool readerEntered = false;
            Task readerTask = Task.Run(async () =>
            {
                using (AsyncReaderWriterLock.Releaser r = await rwLock.ReaderLockAsync("reader1"))
                {
                    readerEntered = true;
                }
            });

            await Task.Delay(100);
            Assert.False(readerEntered);
        }
    }

    [Fact]
    public async Task WriterLock_BlocksOtherWriters()
    {
        AsyncReaderWriterLock rwLock = new();
        bool secondWriterEntered = false;

        using (AsyncReaderWriterLock.Releaser w1 = await rwLock.WriterLockAsync("writer1"))
        {
            Task writerTask = Task.Run(async () =>
            {
                using (AsyncReaderWriterLock.Releaser w2 = await rwLock.WriterLockAsync("writer2"))
                {
                    secondWriterEntered = true;
                }
            });

            await Task.Delay(100);
            Assert.False(secondWriterEntered);
        }

        await Task.Delay(100);
        Assert.True(secondWriterEntered);
    }

    [Fact]
    public void TryReaderLock_WhenNoWriter_ReturnsTrue()
    {
        AsyncReaderWriterLock rwLock = new();

        bool acquired = rwLock.TryReaderLock("test", out AsyncReaderWriterLock.Releaser releaser);

        Assert.True(acquired);
        releaser.Dispose();
    }

    [Fact]
    public async Task TryWriterLock_WhenReaderHeld_ReturnsFalse()
    {
        AsyncReaderWriterLock rwLock = new();

        using (AsyncReaderWriterLock.Releaser r = await rwLock.ReaderLockAsync("reader"))
        {
            bool acquired = rwLock.TryWriterLock("writer", out _);
            Assert.False(acquired);
        }
    }

    [Fact]
    public void TryWriterLock_WhenFree_ReturnsTrue()
    {
        AsyncReaderWriterLock rwLock = new();

        bool acquired = rwLock.TryWriterLock("writer", out AsyncReaderWriterLock.Releaser releaser);

        Assert.True(acquired);
        releaser.Dispose();
    }
}
