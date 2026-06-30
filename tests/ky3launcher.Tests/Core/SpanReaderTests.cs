using Launcher.Core;
using Xunit;

namespace ky3launcher.Tests.Core;

public class SpanReaderTests
{
    [Fact]
    public void TryReadTo_DelimiterFound_SplitsCorrectly()
    {
        SpanReader<char> reader = new("key=value".AsSpan());
        bool found = reader.TryReadTo('=', out ReadOnlySpan<char> span);
        Assert.True(found);
        Assert.Equal("key", span.ToString());
    }

    [Fact]
    public void TryReadTo_DelimiterNotFound_ReturnsFalse()
    {
        SpanReader<char> reader = new("nodelimiter".AsSpan());
        bool found = reader.TryReadTo('=', out _);
        Assert.False(found);
    }

    [Fact]
    public void TryRead_SingleChar_ReturnsFirstChar()
    {
        SpanReader<char> reader = new("abc".AsSpan());
        bool success = reader.TryRead(out char value);
        Assert.True(success);
        Assert.Equal('a', value);
        Assert.Equal(1, reader.Position);
    }

    [Fact]
    public void TryRead_EmptySpan_ReturnsFalse()
    {
        SpanReader<char> reader = new(ReadOnlySpan<char>.Empty);
        bool success = reader.TryRead(out _);
        Assert.False(success);
    }

    [Fact]
    public void TryRead_Count_ReturnsSlice()
    {
        SpanReader<char> reader = new("hello world".AsSpan());
        bool success = reader.TryRead(5, out ReadOnlySpan<char> span);
        Assert.True(success);
        Assert.Equal("hello", span.ToString());
        Assert.Equal(5, reader.Position);
    }

    [Fact]
    public void TryRead_CountExceedsLength_ReturnsFalse()
    {
        SpanReader<char> reader = new("hi".AsSpan());
        bool success = reader.TryRead(10, out _);
        Assert.False(success);
    }

    [Fact]
    public void IsNext_MatchingPrefix_ReturnsTrue()
    {
        SpanReader<char> reader = new("hello".AsSpan());
        Assert.True(reader.IsNext("hel"));
    }

    [Fact]
    public void IsNext_NonMatchingPrefix_ReturnsFalse()
    {
        SpanReader<char> reader = new("hello".AsSpan());
        Assert.False(reader.IsNext("xyz"));
    }

    [Fact]
    public void TryAdvancePast_Matching_AdvancesAndReturnsTrue()
    {
        SpanReader<char> reader = new("prefix:rest".AsSpan());
        bool success = reader.TryAdvancePast("prefix:");
        Assert.True(success);
        Assert.Equal(7, reader.Position);
    }

    [Fact]
    public void TryAdvancePast_NotMatching_ReturnsFalse()
    {
        SpanReader<char> reader = new("hello".AsSpan());
        bool success = reader.TryAdvancePast("xyz");
        Assert.False(success);
        Assert.Equal(0, reader.Position);
    }

    [Fact]
    public void AdvancePast_ConsecutiveValues_AdvancesCorrectly()
    {
        SpanReader<char> reader = new("   hello".AsSpan());
        int count = reader.AdvancePast(' ');
        Assert.Equal(3, count);
        Assert.Equal(3, reader.Position);
    }

    [Fact]
    public void AdvancePast_NoMatch_ReturnsZero()
    {
        SpanReader<char> reader = new("hello".AsSpan());
        int count = reader.AdvancePast(' ');
        Assert.Equal(0, count);
    }

    [Fact]
    public void Advance_MovesPosition()
    {
        SpanReader<char> reader = new("abcdefg".AsSpan());
        reader.Advance(3);
        Assert.Equal(3, reader.Position);
    }

    [Fact]
    public void Rewind_MovesPositionBack()
    {
        SpanReader<char> reader = new("abcdefg".AsSpan());
        reader.Advance(5);
        reader.Rewind(2);
        Assert.Equal(3, reader.Position);
    }

    [Fact]
    public void Reset_MovesToBeginning()
    {
        SpanReader<char> reader = new("abcdefg".AsSpan());
        reader.Advance(5);
        reader.Reset();
        Assert.Equal(0, reader.Position);
    }

    [Fact]
    public void Position_Set_UpdatesUnread()
    {
        SpanReader<char> reader = new("0123456789".AsSpan());
        reader.Position = 5;
        Assert.Equal(5, reader.Position);
        reader.TryRead(out char c);
        Assert.Equal('5', c);
    }

    [Fact]
    public void SequentialReads_ParseKeyValuePairs()
    {
        SpanReader<char> reader = new("name=John&age=30".AsSpan());

        reader.TryReadTo('=', out ReadOnlySpan<char> key1);
        reader.TryReadTo('&', out ReadOnlySpan<char> val1);
        reader.TryReadTo('=', out ReadOnlySpan<char> key2);

        Assert.Equal("name", key1.ToString());
        Assert.Equal("John", val1.ToString());
        Assert.Equal("age", key2.ToString());
    }
}
