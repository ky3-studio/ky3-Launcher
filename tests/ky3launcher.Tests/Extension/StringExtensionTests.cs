using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class StringExtensionTests
{
    [Theory]
    [InlineData("hello", new[] { "hello", "world" }, StringComparison.Ordinal, true)]
    [InlineData("world", new[] { "hello", "world" }, StringComparison.Ordinal, true)]
    [InlineData("foo", new[] { "hello", "world" }, StringComparison.Ordinal, false)]
    [InlineData("HELLO", new[] { "hello", "world" }, StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("HELLO", new[] { "hello", "world" }, StringComparison.Ordinal, false)]
    public void EqualsAny_ReturnsExpected(string value, string[] candidates, StringComparison comparison, bool expected)
    {
        Assert.Equal(expected, value.EqualsAny(candidates, comparison));
    }

    [Fact]
    public void EqualsAny_EmptySpan_ReturnsFalse()
    {
        Assert.False("test".EqualsAny([], StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("/relative/path")]
    public void ToUri_ReturnsValidUri(string input)
    {
        Uri uri = input.ToUri();
        Assert.Equal(input, uri.OriginalString);
    }

    [Theory]
    [InlineData("hello!!!", "!", "hello")]
    [InlineData("foobar", "bar", "foo")]
    [InlineData("test", "xyz", "test")]
    [InlineData("abcabc", "abc", "")]
    public void TrimEnd_RemovesSuffix(string input, string suffix, string expected)
    {
        Assert.Equal(expected, input.TrimEnd(suffix));
    }
}
