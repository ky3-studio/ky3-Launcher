using System.Collections.Immutable;
using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class ImmutableArrayExtensionTests
{
    [Fact]
    public void EmptyIfDefault_Default_ReturnsEmpty()
    {
        ImmutableArray<int> arr = default;
        ImmutableArray<int> result = arr.EmptyIfDefault();
        Assert.True(result.IsEmpty);
        Assert.False(result.IsDefault);
    }

    [Fact]
    public void EmptyIfDefault_NonDefault_ReturnsSame()
    {
        ImmutableArray<int> arr = [1, 2, 3];
        ImmutableArray<int> result = arr.EmptyIfDefault();
        Assert.Equal(arr, result);
    }

    [Fact]
    public void Reverse_ReturnsReversedArray()
    {
        ImmutableArray<int> arr = [1, 2, 3, 4, 5];
        ImmutableArray<int> reversed = arr.Reverse();
        Assert.Equal([5, 4, 3, 2, 1], reversed.ToArray());
    }

    [Fact]
    public void Reverse_Empty_ReturnsEmpty()
    {
        ImmutableArray<int> arr = [];
        ImmutableArray<int> reversed = arr.Reverse();
        Assert.True(reversed.IsEmpty);
    }

    [Fact]
    public void ReverseInPlace_ModifiesArray()
    {
        ImmutableArray<int> arr = [10, 20, 30];
        arr.ReverseInPlace();
        Assert.Equal(30, arr[0]);
        Assert.Equal(20, arr[1]);
        Assert.Equal(10, arr[2]);
    }

    [Fact]
    public void SelectAsArray_TransformsElements()
    {
        ImmutableArray<int> arr = [1, 2, 3];
        ImmutableArray<string> result = arr.SelectAsArray(static x => x.ToString());
        Assert.Equal(["1", "2", "3"], result.ToArray());
    }

    [Fact]
    public void SelectAsArray_Empty_ReturnsEmpty()
    {
        ImmutableArray<int> arr = [];
        ImmutableArray<string> result = arr.SelectAsArray(static x => x.ToString());
        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void SelectAsArray_WithIndex_IncludesIndex()
    {
        ImmutableArray<string> arr = ["a", "b", "c"];
        ImmutableArray<string> result = arr.SelectAsArray(static (item, idx) => $"{idx}:{item}");
        Assert.Equal(["0:a", "1:b", "2:c"], result.ToArray());
    }

    [Fact]
    public void SortInPlace_SortsArray()
    {
        ImmutableArray<int> arr = [3, 1, 4, 1, 5];
        arr.SortInPlace(Comparer<int>.Default);
        Assert.Equal([1, 1, 3, 4, 5], arr.ToArray());
    }

    [Fact]
    public void SortInPlace_Empty_NoOp()
    {
        ImmutableArray<int> arr = [];
        arr.SortInPlace(Comparer<int>.Default);
        Assert.True(arr.IsEmpty);
    }
}
