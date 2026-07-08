using Launcher.Core.Collection.Generic;
using Xunit;

namespace ky3launcher.Tests.Core.Collection;

public class DelegatingPropertyComparerTests
{
    private sealed class ByLengthComparer : DelegatingPropertyComparer<string, int>
    {
        internal ByLengthComparer() : base(static s => s.Length, Comparer<int>.Default) { }
    }

    private readonly ByLengthComparer comparer = new();

    [Fact]
    public void Compare_NullX_NonNullY_ReturnsNegative()
    {
        Assert.Equal(-1, comparer.Compare(null, "abc"));
    }

    [Fact]
    public void Compare_NonNullX_NullY_ReturnsPositive()
    {
        Assert.Equal(1, comparer.Compare("abc", null));
    }

    [Fact]
    public void Compare_BothNull_ReturnsZero()
    {
        Assert.Equal(0, comparer.Compare(null, null));
    }

    [Fact]
    public void Compare_ShorterFirst_ReturnsNegative()
    {
        Assert.True(comparer.Compare("ab", "abc") < 0);
    }

    [Fact]
    public void Compare_LongerFirst_ReturnsPositive()
    {
        Assert.True(comparer.Compare("abc", "ab") > 0);
    }

    [Fact]
    public void Compare_SameLength_ReturnsZero()
    {
        Assert.Equal(0, comparer.Compare("abc", "def"));
    }

    [Fact]
    public void Compare_EmptyStrings_ReturnsZero()
    {
        Assert.Equal(0, comparer.Compare(string.Empty, string.Empty));
    }
}
