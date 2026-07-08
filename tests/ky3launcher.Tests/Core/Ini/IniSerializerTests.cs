using System.Collections.Immutable;
using System.Text;
using Launcher.Core.IO.Ini;
using Xunit;

namespace ky3launcher.Tests.Core.Ini;

public class IniSerializerTests
{
    private static ImmutableArray<IniElement> Parse(string content)
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        return IniSerializer.Deserialize(stream);
    }

    [Fact]
    public void Deserialize_EmptyInput_ReturnsEmpty()
    {
        Assert.Empty(Parse(string.Empty));
    }

    [Fact]
    public void Deserialize_BlankLinesOnly_ReturnsEmpty()
    {
        Assert.Empty(Parse("\n\n\n"));
    }

    [Fact]
    public void Deserialize_SingleParameter_ParsesKeyAndValue()
    {
        ImmutableArray<IniElement> result = Parse("key=value");
        IniParameter param = Assert.Single(result.OfType<IniParameter>());
        Assert.Equal("key", param.Key);
        Assert.Equal("value", param.Value);
    }

    [Fact]
    public void Deserialize_ParameterWithSpaces_TrimsKeyAndValue()
    {
        ImmutableArray<IniElement> result = Parse("  key  =  value  ");
        IniParameter param = Assert.Single(result.OfType<IniParameter>());
        Assert.Equal("key", param.Key);
        Assert.Equal("value", param.Value);
    }

    [Fact]
    public void Deserialize_Comment_ParsesCommentText()
    {
        ImmutableArray<IniElement> result = Parse(";This is a comment");
        IniComment comment = Assert.Single(result.OfType<IniComment>());
        Assert.Equal("This is a comment", comment.Comment);
    }

    [Fact]
    public void Deserialize_Section_ParsesSectionName()
    {
        ImmutableArray<IniElement> result = Parse("[General]");
        IniSection section = Assert.Single(result.OfType<IniSection>());
        Assert.Equal("General", section.Name);
        Assert.Empty(section.Children);
    }

    [Fact]
    public void Deserialize_SectionWithParameters_ChildrenAttachedToSection()
    {
        ImmutableArray<IniElement> result = Parse("[General]\nkey1=val1\nkey2=val2");

        IniSection section = Assert.Single(result.OfType<IniSection>());
        Assert.Equal("General", section.Name);
        Assert.Equal(2, section.Children.Length);

        IniParameter p1 = Assert.IsType<IniParameter>(section.Children[0]);
        Assert.Equal("key1", p1.Key);
        Assert.Equal("val1", p1.Value);
    }

    [Fact]
    public void Deserialize_MultipleSections_AllParsed()
    {
        ImmutableArray<IniElement> result = Parse("[Section1]\nkey=val\n[Section2]\nfoo=bar");

        IniSection[] sections = result.OfType<IniSection>().ToArray();
        Assert.Equal(2, sections.Length);
        Assert.Equal("Section1", sections[0].Name);
        Assert.Equal("Section2", sections[1].Name);
    }

    [Fact]
    public void Deserialize_ValueContainsEquals_PreservesFullValue()
    {
        ImmutableArray<IniElement> result = Parse("url=http://example.com?a=1&b=2");
        IniParameter param = Assert.Single(result.OfType<IniParameter>());
        Assert.Equal("url", param.Key);
        Assert.Equal("http://example.com?a=1&b=2", param.Value);
    }

    [Fact]
    public void SerializeToFile_ThenDeserialize_RoundTrip()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            IniSection section = new("General", [new IniParameter("key", "value")]);
            IniSerializer.SerializeToFile(tempFile, [section, .. section.Children]);

            ImmutableArray<IniElement> result = IniSerializer.DeserializeFromFile(tempFile);
            IniSection restored = Assert.Single(result.OfType<IniSection>());
            Assert.Equal("General", restored.Name);

            IniParameter param = Assert.IsType<IniParameter>(restored.Children[0]);
            Assert.Equal("key", param.Key);
            Assert.Equal("value", param.Value);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
