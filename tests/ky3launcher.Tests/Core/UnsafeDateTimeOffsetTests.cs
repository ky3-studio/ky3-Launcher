using Launcher.Core;
using Xunit;

namespace ky3launcher.Tests.Core;

public class UnsafeDateTimeOffsetTests
{
    [Fact]
    public void FromUnixTimeRelaxed_NullTimestamp_ReturnsDefault()
    {
        DateTimeOffset defaultValue = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(null, defaultValue);
        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void FromUnixTimeRelaxed_ValidSeconds_ReturnsCorrectTime()
    {
        long timestamp = 1704067200;
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(timestamp, default);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void FromUnixTimeRelaxed_ValidMilliseconds_ReturnsCorrectTime()
    {
        long timestamp = 1704067200000;
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(timestamp, default);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void FromUnixTimeRelaxed_Zero_ReturnsUnixEpoch()
    {
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(0L, default);
        Assert.Equal(DateTimeOffset.UnixEpoch, result);
    }

    [Fact]
    public void FromUnixTimeRelaxed_NegativeValidSeconds_ReturnsCorrectTime()
    {
        long timestamp = -86400;
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(timestamp, default);
        Assert.Equal(new DateTimeOffset(1969, 12, 31, 0, 0, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void FromUnixTimeRelaxed_OutOfRange_ReturnsDefault()
    {
        DateTimeOffset defaultValue = new(2000, 6, 15, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(long.MaxValue, defaultValue);
        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseDateTime_ValidDateString_ReturnsCorrectOffset()
    {
        TimeSpan offset = TimeSpan.FromHours(8);
        DateTimeOffset result = UnsafeDateTimeOffset.ParseDateTime("2024-06-15 12:30:00", offset);
        Assert.Equal(2024, result.Year);
        Assert.Equal(6, result.Month);
        Assert.Equal(15, result.Day);
        Assert.Equal(12, result.Hour);
        Assert.Equal(30, result.Minute);
        Assert.Equal(offset, result.Offset);
    }

    [Fact]
    public void FromUnixTimeRelaxed_BoundarySeconds_MaxValid()
    {
        long timestamp = 253402300799L;
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(timestamp, default);
        Assert.Equal(9999, result.Year);
    }

    [Fact]
    public void FromUnixTimeRelaxed_BoundaryMilliseconds_MinValid()
    {
        long timestamp = -62135596800000L;
        DateTimeOffset result = UnsafeDateTimeOffset.FromUnixTimeRelaxed(timestamp, default);
        Assert.Equal(1, result.Year);
    }
}
