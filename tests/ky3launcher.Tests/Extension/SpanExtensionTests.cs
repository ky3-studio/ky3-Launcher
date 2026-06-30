using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class SpanExtensionTests
{
    [Fact]
    public void After_SeparatorExists_ReturnsContentAfter()
    {
        ReadOnlySpan<char> span = "hello:world".AsSpan();
        string result = span.After(':').ToString();
        Assert.Equal("world", result);
    }

    [Fact]
    public void After_SeparatorNotFound_ReturnsOriginal()
    {
        ReadOnlySpan<char> span = "helloworld".AsSpan();
        string result = span.After(':').ToString();
        Assert.Equal("helloworld", result);
    }

    [Fact]
    public void Before_SeparatorExists_ReturnsContentBefore()
    {
        ReadOnlySpan<char> span = "hello:world".AsSpan();
        string result = span.Before(':').ToString();
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Before_SeparatorNotFound_ReturnsOriginal()
    {
        ReadOnlySpan<char> span = "helloworld".AsSpan();
        string result = span.Before(':').ToString();
        Assert.Equal("helloworld", result);
    }

    [Fact]
    public void TrySplitIntoTwo_ValidSeparator_SplitsCorrectly()
    {
        ReadOnlySpan<char> span = "key=value".AsSpan();
        bool success = span.TrySplitIntoTwo('=', out ReadOnlySpan<char> left, out ReadOnlySpan<char> right);
        Assert.True(success);
        Assert.Equal("key", left.ToString());
        Assert.Equal("value", right.ToString());
    }

    [Fact]
    public void TrySplitIntoTwo_NoSeparator_ReturnsFalse()
    {
        ReadOnlySpan<char> span = "noseparator".AsSpan();
        bool success = span.TrySplitIntoTwo('=', out _, out _);
        Assert.False(success);
    }

    [Fact]
    public void BinarySearch_ElementExists_ReturnsElement()
    {
        string[] items = ["alpha", "beta", "gamma", "delta", "epsilon"];
        Array.Sort(items, StringComparer.Ordinal);
        ReadOnlySpan<string> span = items.AsSpan();

        string? result = span.BinarySearch("gamma", (item, current) => string.Compare(item, current, StringComparison.Ordinal));
        Assert.Equal("gamma", result);
    }

    [Fact]
    public void BinarySearch_ElementNotExists_ReturnsNull()
    {
        string[] items = ["alpha", "beta", "gamma"];
        Array.Sort(items, StringComparer.Ordinal);
        ReadOnlySpan<string> span = items.AsSpan();

        string? result = span.BinarySearch("zeta", (item, current) => string.Compare(item, current, StringComparison.Ordinal));
        Assert.Null(result);
    }

    [Fact]
    public void BinarySearch_EmptySpan_ReturnsNull()
    {
        ReadOnlySpan<string> span = [];
        string? result = span.BinarySearch("any", (item, current) => string.Compare(item, current, StringComparison.Ordinal));
        Assert.Null(result);
    }

    [Fact]
    public void IndexOfMax_MultipleElements_ReturnsMaxIndex()
    {
        ReadOnlySpan<int> span = [3, 7, 2, 9, 5];
        int index = span.IndexOfMax();
        Assert.Equal(3, index);
    }

    [Fact]
    public void IndexOfMax_FirstIsMax_ReturnsZero()
    {
        ReadOnlySpan<int> span = [99, 1, 2, 3];
        int index = span.IndexOfMax();
        Assert.Equal(0, index);
    }

    [Fact]
    public void Average_ByteSpan_ReturnsCorrectAverage()
    {
        ReadOnlySpan<byte> span = [10, 20, 30, 40];
        byte avg = span.Average();
        Assert.Equal(25, avg);
    }

    [Fact]
    public void Average_EmptySpan_ReturnsZero()
    {
        ReadOnlySpan<byte> span = [];
        byte avg = span.Average();
        Assert.Equal(0, avg);
    }

    [Fact]
    public void Average_SingleElement_ReturnsThatElement()
    {
        ReadOnlySpan<byte> span = [42];
        byte avg = span.Average();
        Assert.Equal(42, avg);
    }
}
