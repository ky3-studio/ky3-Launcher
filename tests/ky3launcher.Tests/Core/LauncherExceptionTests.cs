using Launcher.Core.ExceptionService;
using Xunit;

namespace ky3launcher.Tests.Core;

public class LauncherExceptionTests
{
    [Fact]
    public void Throw_AlwaysThrowsLauncherException()
    {
        Assert.Throws<LauncherException>(() => LauncherException.Throw("test message"));
    }

    [Fact]
    public void Throw_MessageContainsOriginalMessage()
    {
        LauncherException ex = Assert.Throws<LauncherException>(
            () => LauncherException.Throw("test message"));
        Assert.Contains("test message", ex.Message);
    }

    [Fact]
    public void Throw_WithInnerException_WrapsAndPreservesInner()
    {
        InvalidOperationException inner = new("inner msg");
        LauncherException ex = Assert.Throws<LauncherException>(
            () => LauncherException.Throw("outer", inner));
        Assert.Same(inner, ex.InnerException);
        Assert.Contains("inner msg", ex.Message);
    }

    [Fact]
    public void ThrowIf_True_Throws()
    {
        Assert.Throws<LauncherException>(() => LauncherException.ThrowIf(true, "msg"));
    }

    [Fact]
    public void ThrowIf_False_DoesNotThrow()
    {
        LauncherException.ThrowIf(false, "msg");
    }

    [Fact]
    public void ThrowIfNot_False_Throws()
    {
        Assert.Throws<LauncherException>(() => LauncherException.ThrowIfNot(false, "msg"));
    }

    [Fact]
    public void ThrowIfNot_True_DoesNotThrow()
    {
        LauncherException.ThrowIfNot(true, "msg");
    }

    [Fact]
    public void Argument_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => LauncherException.Argument("bad param", "paramName"));
        Assert.Equal("paramName", ex.ParamName);
    }

    [Fact]
    public void InvalidOperation_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => LauncherException.InvalidOperation("msg"));
    }

    [Fact]
    public void NotSupported_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => LauncherException.NotSupported());
    }

    [Fact]
    public void NotSupportedIf_True_Throws()
    {
        Assert.Throws<NotSupportedException>(() => LauncherException.NotSupportedIf(true));
    }

    [Fact]
    public void NotSupportedIf_False_DoesNotThrow()
    {
        LauncherException.NotSupportedIf(false);
    }

    [Fact]
    public void InvalidCast_ThrowsInvalidCastException()
    {
        Assert.Throws<InvalidCastException>(
            () => LauncherException.InvalidCast<string, int>("myName"));
    }

    [Fact]
    public void IO_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => LauncherException.IO("disk error"));
    }

    [Fact]
    public void OperationCanceled_ThrowsOperationCanceledException()
    {
        Assert.Throws<OperationCanceledException>(
            () => LauncherException.OperationCanceled("canceled"));
    }
}
