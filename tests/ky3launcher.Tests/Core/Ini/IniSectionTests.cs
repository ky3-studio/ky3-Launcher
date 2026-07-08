using System.Collections.Immutable;
using Launcher.Core.IO.Ini;
using Xunit;

namespace ky3launcher.Tests.Core.Ini;

public class IniSectionTests
{
    [Fact]
    public void ToString_FormatsBracketsAroundName()
    {
        IniSection section = new("General", ImmutableArray<IniElement>.Empty);
        Assert.Equal("[General]", section.ToString());
    }

    [Fact]
    public void Equals_SameNameAndChildren_ReturnsTrue()
    {
        ImmutableArray<IniElement> children = [new IniParameter("k", "v")];
        IniSection a = new("Name", children);
        IniSection b = new("Name", children);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentName_ReturnsFalse()
    {
        IniSection a = new("Name1", ImmutableArray<IniElement>.Empty);
        IniSection b = new("Name2", ImmutableArray<IniElement>.Empty);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentChildren_ReturnsFalse()
    {
        ImmutableArray<IniElement> children1 = [new IniParameter("k", "v1")];
        ImmutableArray<IniElement> children2 = [new IniParameter("k", "v2")];
        IniSection a = new("Name", children1);
        IniSection b = new("Name", children2);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Builder_ToSection_CreatesCorrectSection()
    {
        IniSection.Builder builder = new("Config");
        builder.Add(new IniParameter("key1", "val1"));
        builder.Add(new IniComment("a comment"));
        IniSection section = builder.ToSection();

        Assert.Equal("Config", section.Name);
        Assert.Equal(2, section.Children.Length);
        Assert.IsType<IniParameter>(section.Children[0]);
        Assert.IsType<IniComment>(section.Children[1]);
    }

    [Fact]
    public void Builder_ToSection_EmptyChildren_IsEmpty()
    {
        IniSection.Builder builder = new("Empty");
        IniSection section = builder.ToSection();
        Assert.Equal("Empty", section.Name);
        Assert.Empty(section.Children);
    }

    [Fact]
    public void Builder_ToString_ThrowsNotSupportedException()
    {
        IniSection.Builder builder = new("Name");
        Assert.Throws<NotSupportedException>(() => builder.ToString());
    }
}
