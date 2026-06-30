using Launcher.Extension;
using System.Text;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class StringBuilderExtensionTests
{
    [Fact]
    public void AppendIf_True_AppendsChar()
    {
        StringBuilder sb = new("hello");
        sb.AppendIf(true, '!');
        Assert.Equal("hello!", sb.ToString());
    }

    [Fact]
    public void AppendIf_False_DoesNotAppendChar()
    {
        StringBuilder sb = new("hello");
        sb.AppendIf(false, '!');
        Assert.Equal("hello", sb.ToString());
    }

    [Fact]
    public void AppendIf_True_AppendsString()
    {
        StringBuilder sb = new("hello");
        sb.AppendIf(true, " world");
        Assert.Equal("hello world", sb.ToString());
    }

    [Fact]
    public void AppendIf_False_DoesNotAppendString()
    {
        StringBuilder sb = new("hello");
        sb.AppendIf(false, " world");
        Assert.Equal("hello", sb.ToString());
    }

    [Theory]
    [InlineData("hello\r\n\r\n", "hello")]
    [InlineData("content\n  \n", "content")]
    [InlineData("no trailing", "no trailing")]
    [InlineData("\r\n", "")]
    [InlineData("", "")]
    public void ToStringTrimEndNewLine_TrimsCorrectly(string input, string expected)
    {
        StringBuilder sb = new(input);
        Assert.Equal(expected, sb.ToStringTrimEndNewLine());
    }
}
