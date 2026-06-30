using Launcher.Core;
using Xunit;

namespace ky3launcher.Tests.Core;

public class CommandLineBuilderTests
{
    [Fact]
    public void Empty_ReturnsEmptyString()
    {
        CommandLineBuilder builder = new();
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void SingleOption_NoValue_ReturnsOptionName()
    {
        CommandLineBuilder builder = new();
        builder.Append("-verbose");
        Assert.Equal(" -verbose", builder.ToString());
    }

    [Fact]
    public void SingleOption_WithValue_WhiteSpaceSeparator()
    {
        CommandLineBuilder builder = new();
        builder.Append("-output", "file.txt");
        Assert.Equal(" -output file.txt", builder.ToString());
    }

    [Fact]
    public void SingleOption_WithValue_EqualSeparator()
    {
        CommandLineBuilder builder = new();
        builder.Append("--level", "5", CommandLineArgumentPrefix.Equal);
        Assert.Equal(" --level=5", builder.ToString());
    }

    [Fact]
    public void MultipleOptions_ConcatenatesWithSpace()
    {
        CommandLineBuilder builder = new();
        builder.Append("-a").Append("-b", "value").Append("-c");
        Assert.Equal(" -a -b value -c", builder.ToString());
    }

    [Fact]
    public void AppendIf_ConditionTrue_AppendsOption()
    {
        CommandLineBuilder builder = new();
        builder.AppendIf(true, "--flag");
        Assert.Equal(" --flag", builder.ToString());
    }

    [Fact]
    public void AppendIf_ConditionFalse_SkipsOption()
    {
        CommandLineBuilder builder = new();
        builder.AppendIf(false, "--flag");
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void AppendIfNotNull_ValueNotNull_AppendsOption()
    {
        CommandLineBuilder builder = new();
        builder.AppendIfNotNull("--path", "/usr/bin");
        Assert.Equal(" --path /usr/bin", builder.ToString());
    }

    [Fact]
    public void AppendIfNotNull_ValueNull_SkipsOption()
    {
        CommandLineBuilder builder = new();
        builder.AppendIfNotNull("--path", null);
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void FluentChaining_BuildsComplexCommandLine()
    {
        string result = new CommandLineBuilder()
            .Append("launch")
            .Append("-game", "genshin")
            .AppendIf(true, "--fullscreen")
            .AppendIf(false, "--windowed")
            .Append("--fps", 60, CommandLineArgumentPrefix.Equal)
            .ToString();

        Assert.Equal(" launch -game genshin --fullscreen --fps=60", result);
    }
}
