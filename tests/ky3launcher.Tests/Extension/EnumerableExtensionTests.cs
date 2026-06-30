using System.Collections.ObjectModel;
using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class EnumerableExtensionTests
{
    [Fact]
    public void ToObservableCollection_ConvertsCorrectly()
    {
        IEnumerable<int> source = [1, 2, 3];
        ObservableCollection<int> result = source.ToObservableCollection();
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0]);
        Assert.Equal(3, result[2]);
    }

    [Fact]
    public void ToObservableCollection_Empty_ReturnsEmptyCollection()
    {
        IEnumerable<int> source = [];
        ObservableCollection<int> result = source.ToObservableCollection();
        Assert.Empty(result);
    }

    [Fact]
    public void ToString_JoinsWithSeparator()
    {
        IEnumerable<int> source = [1, 2, 3];
        string result = source.ToString(',');
        Assert.Equal("1,2,3", result);
    }

    [Fact]
    public void ToString_SingleElement_NoSeparator()
    {
        IEnumerable<string> source = ["hello"];
        string result = source.ToString('|');
        Assert.Equal("hello", result);
    }

    [Fact]
    public void ToString_Empty_ReturnsEmpty()
    {
        IEnumerable<int> source = [];
        string result = source.ToString(',');
        Assert.Equal(string.Empty, result);
    }
}
