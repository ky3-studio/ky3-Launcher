using System.Collections.Immutable;
using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class ImmutableDictionaryExtensionTests
{
    [Fact]
    public void ToImmutableDictionaryIgnoringDuplicateKeys_NoDuplicates_AllPresent()
    {
        List<string> source = ["a", "bb", "ccc"];
        ImmutableDictionary<int, string> dict = source.ToImmutableDictionaryIgnoringDuplicateKeys(s => s.Length);
        Assert.Equal(3, dict.Count);
    }

    [Fact]
    public void ToImmutableDictionaryIgnoringDuplicateKeys_Duplicates_LastWins()
    {
        List<string> source = ["ab", "cd", "ef"];
        ImmutableDictionary<int, string> dict = source.ToImmutableDictionaryIgnoringDuplicateKeys(s => s.Length);
        Assert.Single(dict);
        Assert.Equal("ef", dict[2]);
    }

    [Fact]
    public void ToImmutableDictionaryIgnoringDuplicateKeys_Empty_ReturnsEmpty()
    {
        List<string> source = [];
        ImmutableDictionary<int, string> dict = source.ToImmutableDictionaryIgnoringDuplicateKeys(s => s.Length);
        Assert.Empty(dict);
    }

    [Fact]
    public void ToImmutableDictionaryIgnoringDuplicateKeys_WithValueSelector()
    {
        List<string> source = ["hello", "world"];
        ImmutableDictionary<string, int> dict = source.ToImmutableDictionaryIgnoringDuplicateKeys(s => s, s => s.Length);
        Assert.Equal(5, dict["hello"]);
        Assert.Equal(5, dict["world"]);
    }
}
