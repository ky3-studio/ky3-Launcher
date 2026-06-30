using System.Collections.ObjectModel;
using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class CollectionExtensionTests
{
    [Fact]
    public void RemoveWhere_RemovesMatchingItems_ReturnsCount()
    {
        Collection<int> collection = [1, 2, 3, 4, 5, 6];
        int removed = collection.RemoveWhere(x => x % 2 == 0);
        Assert.Equal(3, removed);
        Assert.Equal([1, 3, 5], collection);
    }

    [Fact]
    public void RemoveWhere_NoneMatch_ReturnsZero()
    {
        Collection<int> collection = [1, 3, 5];
        int removed = collection.RemoveWhere(x => x > 100);
        Assert.Equal(0, removed);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void RemoveWhere_AllMatch_RemovesAll()
    {
        Collection<int> collection = [2, 4, 6];
        int removed = collection.RemoveWhere(x => x % 2 == 0);
        Assert.Equal(3, removed);
        Assert.Empty(collection);
    }

    [Fact]
    public void FirstIndexOf_ElementExists_ReturnsCorrectIndex()
    {
        Collection<string> collection = ["a", "b", "c", "d"];
        int index = collection.FirstIndexOf(x => x == "c");
        Assert.Equal(2, index);
    }

    [Fact]
    public void FirstIndexOf_ElementNotExists_ReturnsNegativeOne()
    {
        Collection<string> collection = ["a", "b", "c"];
        int index = collection.FirstIndexOf(x => x == "z");
        Assert.Equal(-1, index);
    }

    [Fact]
    public void FirstIndexOf_MultipleMatches_ReturnsFirst()
    {
        Collection<int> collection = [10, 20, 30, 20, 10];
        int index = collection.FirstIndexOf(x => x == 20);
        Assert.Equal(1, index);
    }

    [Fact]
    public void FirstIndexOf_EmptyCollection_ReturnsNegativeOne()
    {
        Collection<int> collection = [];
        int index = collection.FirstIndexOf(x => x > 0);
        Assert.Equal(-1, index);
    }
}
