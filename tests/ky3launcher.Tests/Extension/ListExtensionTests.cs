using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class ListExtensionTests
{
    [Fact]
    public void Average_NonEmpty_ReturnsCorrectAverage()
    {
        List<int> list = [10, 20, 30];
        Assert.Equal(20.0, list.Average());
    }

    [Fact]
    public void Average_Empty_ReturnsZero()
    {
        List<int> list = [];
        Assert.Equal(0.0, list.Average());
    }

    [Fact]
    public void Average_SingleElement_ReturnsThatElement()
    {
        List<int> list = [42];
        Assert.Equal(42.0, list.Average());
    }

    [Fact]
    public void Average_LargeValues_NoOverflow()
    {
        List<int> list = [int.MaxValue, int.MaxValue];
        double expected = (long)int.MaxValue * 2.0 / 2;
        Assert.Equal(expected, list.Average());
    }

    [Fact]
    public void GetRange_ValidRange_ReturnsSubList()
    {
        List<int> list = [1, 2, 3, 4, 5];
        List<int> result = list.GetRange(1..4);
        Assert.Equal([2, 3, 4], result);
    }

    [Fact]
    public void GetRange_FromEnd_ReturnsCorrectSlice()
    {
        List<int> list = [10, 20, 30, 40, 50];
        List<int> result = list.GetRange(^2..);
        Assert.Equal([40, 50], result);
    }

    [Fact]
    public void SortBy_SortsAscending()
    {
        List<string> list = ["banana", "apple", "cherry"];
        list.SortBy(s => s.Length);
        Assert.Equal("apple", list[0]);
        Assert.Equal("banana", list[1]);
        Assert.Equal("cherry", list[2]);
    }

    [Fact]
    public void SortByDescending_SortsDescending()
    {
        List<string> list = ["a", "ccc", "bb"];
        list.SortByDescending(s => s.Length);
        Assert.Equal("ccc", list[0]);
        Assert.Equal("bb", list[1]);
        Assert.Equal("a", list[2]);
    }

    [Fact]
    public void RemoveLast_RemovesLastElement()
    {
        List<int> list = [1, 2, 3];
        ((IList<int>)list).RemoveLast();
        Assert.Equal([1, 2], list);
    }

    [Fact]
    public void BinarySearch_Found_ReturnsItem()
    {
        List<string> list = ["alpha", "beta", "gamma"];
        list.Sort(StringComparer.Ordinal);
        string? result = list.BinarySearch("beta", (item, current) => string.Compare(item, current, StringComparison.Ordinal));
        Assert.Equal("beta", result);
    }

    [Fact]
    public void BinarySearch_NotFound_ReturnsNull()
    {
        List<string> list = ["alpha", "beta", "gamma"];
        list.Sort(StringComparer.Ordinal);
        string? result = list.BinarySearch("delta", (item, current) => string.Compare(item, current, StringComparison.Ordinal));
        Assert.Null(result);
    }
}
