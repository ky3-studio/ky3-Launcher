using Launcher.Core.IO;
using Xunit;

namespace ky3launcher.Tests.Core;

public class ValueDirectoryTests
{
    [Fact]
    public void HasValue_WithNonNullString_ReturnsTrue()
    {
        ValueDirectory dir = @"C:\Users\Test";
        Assert.True(dir.HasValue);
    }

    [Fact]
    public void HasValue_WithNullString_ReturnsFalse()
    {
        ValueDirectory dir = (string?)null!;
        Assert.False(dir.HasValue);
    }

    [Fact]
    public void ImplicitToString_ReturnsOriginalPath()
    {
        ValueDirectory dir = @"C:\Users\Test";
        string path = dir;
        Assert.Equal(@"C:\Users\Test", path);
    }

    [Fact]
    public void ToString_ReturnsPath()
    {
        ValueDirectory dir = @"C:\Users\Test";
        Assert.Equal(@"C:\Users\Test", dir.ToString());
    }

    [Fact]
    public void Equals_SamePath_ReturnsTrue()
    {
        ValueDirectory a = @"C:\Users\Test";
        ValueDirectory b = @"C:\Users\Test";
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentPath_ReturnsFalse()
    {
        ValueDirectory a = @"C:\Users\Test1";
        ValueDirectory b = @"C:\Users\Test2";
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_CaseSensitive_ReturnsFalse()
    {
        ValueDirectory a = @"C:\users\test";
        ValueDirectory b = @"C:\Users\Test";
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void GetHashCode_SamePath_SameHash()
    {
        ValueDirectory a = @"C:\Users\Test";
        ValueDirectory b = @"C:\Users\Test";
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
