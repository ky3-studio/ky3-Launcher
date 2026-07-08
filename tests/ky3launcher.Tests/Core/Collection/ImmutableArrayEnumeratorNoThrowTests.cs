using System.Collections.Immutable;
using Launcher.Core.Collection.Generic;
using Xunit;

namespace ky3launcher.Tests.Core.Collection;

public class ImmutableArrayEnumeratorNoThrowTests
{
    [Fact]
    public void MoveNext_EmptyArray_ReturnsFalseImmediately()
    {
        using ImmutableArrayEnumeratorNoThrow<int> enumerator = new(ImmutableArray<int>.Empty);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void MoveNext_SingleElement_ReturnsTrueThenFalse()
    {
        using ImmutableArrayEnumeratorNoThrow<int> enumerator = new([42]);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(42, enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void MoveNext_MultipleElements_IteratesInOrder()
    {
        ImmutableArray<string> array = ["a", "b", "c"];
        using ImmutableArrayEnumeratorNoThrow<string> enumerator = new(array);

        List<string> results = [];
        while (enumerator.MoveNext())
        {
            results.Add(enumerator.Current);
        }

        Assert.Equal(["a", "b", "c"], results);
    }

    [Fact]
    public void Current_AfterExhausted_ReturnsDefault()
    {
        using ImmutableArrayEnumeratorNoThrow<int> enumerator = new([1]);
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal(default, enumerator.Current);
    }

    [Fact]
    public void Reset_AllowsReIteration()
    {
        using ImmutableArrayEnumeratorNoThrow<int> enumerator = new([10, 20]);

        enumerator.MoveNext();
        Assert.Equal(10, enumerator.Current);

        enumerator.Reset();

        enumerator.MoveNext();
        Assert.Equal(10, enumerator.Current);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        ImmutableArrayEnumeratorNoThrow<int> enumerator = new([1, 2, 3]);
        enumerator.Dispose();
    }
}
