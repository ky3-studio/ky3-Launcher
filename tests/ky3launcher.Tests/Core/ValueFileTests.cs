using Launcher.Core.IO;
using Xunit;

namespace ky3launcher.Tests.Core;

public class ValueFileTests
{
    [Fact]
    public void ImplicitConversion_FromString_CreatesValueFile()
    {
        ValueFile file = "C:\\test\\file.txt";

        string result = file;

        Assert.Equal("C:\\test\\file.txt", result);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsOriginal()
    {
        ValueFile file = "path/to/file";

        string str = file;

        Assert.Equal("path/to/file", str);
    }

    [Fact]
    public void HasValue_WhenCreatedFromString_ReturnsTrue()
    {
        ValueFile file = "some-file";

        Assert.True(file.HasValue);
    }

    [Fact]
    public void HasValue_WhenCreatedFromNull_ReturnsFalse()
    {
        ValueFile file = (string?)null;

        Assert.False(file.HasValue);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        ValueFile a = "file.txt";
        ValueFile b = "file.txt";

        Assert.True(a.Equals(b));
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        ValueFile a = "file1.txt";
        ValueFile b = "file2.txt";

        Assert.False(a.Equals(b));
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_CaseSensitive()
    {
        ValueFile a = "File.TXT";
        ValueFile b = "file.txt";

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void GetHashCode_SameValue_SameHash()
    {
        ValueFile a = "test";
        ValueFile b = "test";

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsStringValue()
    {
        ValueFile file = "hello.png";

        Assert.Equal("hello.png", file.ToString());
    }

    [Fact]
    public void Equals_Object_WorksCorrectly()
    {
        ValueFile a = "x";
        object b = (ValueFile)"x";
        object c = "x";

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
    }
}
