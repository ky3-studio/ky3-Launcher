using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class DictionaryExtensionTests
{
    [Fact]
    public void IncreaseByOne_NewKey_StartsFromZeroAndIncrements()
    {
        Dictionary<string, int> dict = [];

        dict.IncreaseByOne("a");

        Assert.Equal(1, dict["a"]);
    }

    [Fact]
    public void IncreaseByOne_ExistingKey_Increments()
    {
        Dictionary<string, int> dict = new() { ["x"] = 5 };

        dict.IncreaseByOne("x");

        Assert.Equal(6, dict["x"]);
    }

    [Fact]
    public void IncreaseByValue_AddsValue()
    {
        Dictionary<string, int> dict = new() { ["key"] = 10 };

        dict.IncreaseByValue("key", 7);

        Assert.Equal(17, dict["key"]);
    }

    [Fact]
    public void IncreaseByValue_NewKey_StartsFromDefault()
    {
        Dictionary<string, int> dict = [];

        dict.IncreaseByValue("new", 42);

        Assert.Equal(42, dict["new"]);
    }

    [Fact]
    public void DecreaseByValue_SubtractsValue()
    {
        Dictionary<string, int> dict = new() { ["key"] = 10 };

        dict.DecreaseByValue("key", 3);

        Assert.Equal(7, dict["key"]);
    }

    [Fact]
    public void TryIncreaseByOne_ExistingKey_ReturnsTrue()
    {
        Dictionary<string, int> dict = new() { ["a"] = 0 };

        bool result = dict.TryIncreaseByOne("a");

        Assert.True(result);
        Assert.Equal(1, dict["a"]);
    }

    [Fact]
    public void TryIncreaseByOne_MissingKey_ReturnsFalse()
    {
        Dictionary<string, int> dict = [];

        bool result = dict.TryIncreaseByOne("missing");

        Assert.False(result);
        Assert.False(dict.ContainsKey("missing"));
    }

    [Fact]
    public void ToDictionaryIgnoringDuplicateKeys_WithDuplicates_LastWins()
    {
        List<(string Name, int Value)> source =
        [
            ("a", 1),
            ("b", 2),
            ("a", 3),
        ];

        Dictionary<string, (string Name, int Value)> result =
            source.ToDictionaryIgnoringDuplicateKeys(x => x.Name);

        Assert.Equal(2, result.Count);
        Assert.Equal(3, result["a"].Value);
        Assert.Equal(2, result["b"].Value);
    }

    [Fact]
    public void ToDictionaryIgnoringDuplicateKeys_WithValueSelector_Works()
    {
        List<(string Key, int Val)> source = [("x", 10), ("y", 20), ("x", 30)];

        Dictionary<string, int> result =
            source.ToDictionaryIgnoringDuplicateKeys(x => x.Key, x => x.Val);

        Assert.Equal(30, result["x"]);
        Assert.Equal(20, result["y"]);
    }

    [Fact]
    public void WithKeysRemoved_RemovesSpecifiedKeys()
    {
        IDictionary<string, int> dict = new Dictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            ["d"] = 4,
        };
        HashSet<string> keysToRemove = ["b", "d"];

        IDictionary<string, int> result = dict.WithKeysRemoved(keysToRemove);

        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey("a"));
        Assert.True(result.ContainsKey("c"));
        Assert.False(result.ContainsKey("b"));
        Assert.False(result.ContainsKey("d"));
    }

    [Fact]
    public void WithKeysRemoved_EmptyRemoveSet_ReturnsAll()
    {
        IDictionary<string, int> dict = new Dictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2,
        };
        HashSet<string> empty = [];

        IDictionary<string, int> result = dict.WithKeysRemoved(empty);

        Assert.Equal(2, result.Count);
    }
}
