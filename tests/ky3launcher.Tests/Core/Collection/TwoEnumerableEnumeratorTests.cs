using System.Collections.Immutable;
using Launcher.Core.Collection.Generic;
using Xunit;

namespace ky3launcher.Tests.Core.Collection;

public class TwoEnumerableEnumeratorTests
{
    [Fact]
    public void MoveNext_BothEmpty_ReturnsFalse()
    {
        using TwoEnumerableEnumerator<int, int> enumerator = new([], []);
        bool moveFirst = true;
        bool moveSecond = true;
        Assert.False(enumerator.MoveNext(ref moveFirst, ref moveSecond));
    }

    [Fact]
    public void MoveNext_EqualLength_IteratesBoth()
    {
        using TwoEnumerableEnumerator<int, string> enumerator = new([1, 2, 3], ["a", "b", "c"]);
        bool moveFirst = true;
        bool moveSecond = true;

        List<(int, string)> results = [];
        while (enumerator.MoveNext(ref moveFirst, ref moveSecond))
        {
            results.Add((enumerator.Current.First!, enumerator.Current.Second!));
        }

        Assert.Equal(3, results.Count);
        Assert.Equal((1, "a"), results[0]);
        Assert.Equal((3, "c"), results[2]);
        Assert.False(moveFirst);
        Assert.False(moveSecond);
    }

    [Fact]
    public void MoveNext_FirstLonger_ContinuesAfterSecondExhausted()
    {
        using TwoEnumerableEnumerator<int, int> enumerator = new([1, 2, 3], [10]);
        bool moveFirst = true;
        bool moveSecond = true;

        int iterations = 0;
        while (enumerator.MoveNext(ref moveFirst, ref moveSecond))
        {
            iterations++;
        }

        Assert.Equal(3, iterations);
        Assert.False(moveFirst);
        Assert.False(moveSecond);
    }

    [Fact]
    public void MoveNext_SecondLonger_ContinuesAfterFirstExhausted()
    {
        using TwoEnumerableEnumerator<int, int> enumerator = new([10], [1, 2, 3]);
        bool moveFirst = true;
        bool moveSecond = true;

        int iterations = 0;
        while (enumerator.MoveNext(ref moveFirst, ref moveSecond))
        {
            iterations++;
        }

        Assert.Equal(3, iterations);
    }

    [Fact]
    public void MoveNext_WithImmutableArray_UsesNoThrowEnumerator()
    {
        ImmutableArray<int> first = [1, 2];
        ImmutableArray<string> second = ["x", "y"];

        using TwoEnumerableEnumerator<int, string> enumerator = new(first, second);
        bool moveFirst = true;
        bool moveSecond = true;

        enumerator.MoveNext(ref moveFirst, ref moveSecond);
        Assert.Equal((1, "x"), (enumerator.Current.First, enumerator.Current.Second));
    }

    [Fact]
    public void MoveNext_OnceMoveFalse_StaysExhausted()
    {
        using TwoEnumerableEnumerator<int, int> enumerator = new([1], [1, 2]);
        bool moveFirst = true;
        bool moveSecond = true;

        enumerator.MoveNext(ref moveFirst, ref moveSecond);
        enumerator.MoveNext(ref moveFirst, ref moveSecond);

        Assert.False(moveFirst);
        Assert.True(moveSecond);
    }
}
