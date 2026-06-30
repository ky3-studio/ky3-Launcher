using Xunit;
using LauncherRandom = Launcher.Core.Random;

namespace ky3launcher.Tests.Core;

public class RandomTests
{
    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void GetLowerHexString_ReturnsCorrectLength(int length)
    {
        string result = LauncherRandom.GetLowerHexString(length);
        Assert.Equal(length, result.Length);
    }

    [Fact]
    public void GetLowerHexString_ContainsOnlyHexChars()
    {
        string result = LauncherRandom.GetLowerHexString(100);
        Assert.All(result, c => Assert.Contains(c, "0123456789abcdef"));
    }

    [Fact]
    public void GetUpperAndNumberString_ReturnsCorrectLength()
    {
        string result = LauncherRandom.GetUpperAndNumberString(24);
        Assert.Equal(24, result.Length);
    }

    [Fact]
    public void GetUpperAndNumberString_ContainsOnlyValidChars()
    {
        string result = LauncherRandom.GetUpperAndNumberString(100);
        Assert.All(result, c => Assert.Contains(c, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
    }

    [Fact]
    public void GetLowerAndNumberString_ReturnsCorrectLength()
    {
        string result = LauncherRandom.GetLowerAndNumberString(20);
        Assert.Equal(20, result.Length);
    }

    [Fact]
    public void GetLowerAndNumberString_ContainsOnlyValidChars()
    {
        string result = LauncherRandom.GetLowerAndNumberString(100);
        Assert.All(result, c => Assert.Contains(c, "0123456789abcdefghijklmnopqrstuvwxyz"));
    }

    [Fact]
    public void GetLowerHexString_SpanOverload_FillsBuffer()
    {
        Span<char> buffer = stackalloc char[16];
        LauncherRandom.GetLowerHexString(buffer);
        string result = buffer.ToString();
        Assert.Equal(16, result.Length);
        Assert.All(result, c => Assert.Contains(c, "0123456789abcdef"));
    }

    [Fact]
    public void TwoConsecutiveCalls_ProduceDifferentResults()
    {
        string result1 = LauncherRandom.GetLowerHexString(32);
        string result2 = LauncherRandom.GetLowerHexString(32);
        Assert.NotEqual(result1, result2);
    }
}
