using Launcher.Core.IO.Ini;
using Xunit;

namespace ky3launcher.Tests.Core.Ini;

public class IniCommentTests
{
    [Fact]
    public void ToString_PrependsSemicolon()
    {
        IniComment comment = new("This is a comment");
        Assert.Equal(";This is a comment", comment.ToString());
    }

    [Fact]
    public void ToString_EmptyComment_ReturnsSemicolonOnly()
    {
        IniComment comment = new(string.Empty);
        Assert.Equal(";", comment.ToString());
    }

    [Fact]
    public void Equals_SameComment_ReturnsTrue()
    {
        IniComment a = new("comment");
        IniComment b = new("comment");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentComment_ReturnsFalse()
    {
        IniComment a = new("comment1");
        IniComment b = new("comment2");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        IniComment a = new("comment");
        Assert.False(a.Equals(null));
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        IniComment a = new("comment");
        Assert.True(a.Equals(a));
    }

    [Fact]
    public void GetHashCode_SameComment_SameHash()
    {
        IniComment a = new("comment");
        IniComment b = new("comment");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
