using Launcher.Core.IO;
using Xunit;

namespace ky3launcher.Tests.Core;

public class StreamCopyStatusTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        StreamCopyStatus status = new(512L, 2048L, 8192L);
        Assert.Equal(512L, status.BytesReadSinceLastReport);
        Assert.Equal(2048L, status.BytesReadSinceCopyStart);
        Assert.Equal(8192L, status.TotalBytes);
    }

    [Fact]
    public void Progress_Calculation_IsCorrect()
    {
        StreamCopyStatus status = new(100L, 500L, 1000L);
        double progress = (double)status.BytesReadSinceCopyStart / status.TotalBytes;
        Assert.Equal(0.5, progress, 10);
    }

    [Fact]
    public void Constructor_ZeroValues_DoesNotThrow()
    {
        StreamCopyStatus status = new(0L, 0L, 0L);
        Assert.Equal(0L, status.BytesReadSinceLastReport);
        Assert.Equal(0L, status.BytesReadSinceCopyStart);
        Assert.Equal(0L, status.TotalBytes);
    }

    [Fact]
    public void Constructor_LargeValues_Preserved()
    {
        StreamCopyStatus status = new(long.MaxValue, long.MaxValue, long.MaxValue);
        Assert.Equal(long.MaxValue, status.BytesReadSinceCopyStart);
    }
}
