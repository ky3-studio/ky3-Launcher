using Launcher.Core;
using Xunit;

namespace ky3launcher.Tests.Core;

public class DisposableMarshalTests
{
    [Fact]
    public void DisposeAndClear_DisposesAndNullifiesReference()
    {
        TestDisposable? obj = new();
        DisposableMarshal.DisposeAndClear(ref obj);
        Assert.Null(obj);
    }

    [Fact]
    public void DisposeAndClear_CallsDispose()
    {
        TestDisposable original = new();
        TestDisposable? obj = original;
        DisposableMarshal.DisposeAndClear(ref obj);
        Assert.True(original.IsDisposed);
    }

    [Fact]
    public void DisposeAndClear_NullReference_NoOp()
    {
        TestDisposable? obj = null;
        DisposableMarshal.DisposeAndClear(ref obj);
        Assert.Null(obj);
    }

    [Fact]
    public void DisposeAndExchange_DisposesOldAndSetsNew()
    {
        TestDisposable old = new();
        TestDisposable replacement = new();
        TestDisposable? obj = old;
        DisposableMarshal.DisposeAndExchange(ref obj, replacement);
        Assert.True(old.IsDisposed);
        Assert.Same(replacement, obj);
        Assert.False(replacement.IsDisposed);
    }

    [Fact]
    public void DisposeAndExchange_WithNull_DisposesOldAndSetsNull()
    {
        TestDisposable old = new();
        TestDisposable? obj = old;
        DisposableMarshal.DisposeAndExchange(ref obj, null);
        Assert.True(old.IsDisposed);
        Assert.Null(obj);
    }

    private sealed class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }
}
