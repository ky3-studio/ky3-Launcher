using System.Net.Http;
using System.Runtime.InteropServices;
using Launcher.Core.ExceptionService;
using Xunit;

namespace ky3launcher.Tests.Core;

public class ExceptionFilterTests
{
    [Fact]
    public void IsKnownWinUiFrameworkException_COMException_0x80073B01_ReturnsTrue()
    {
        COMException exception = new("MUI resource not found", unchecked((int)0x80073B01));
        Assert.True(ExceptionFilter.IsKnownWinUiFrameworkException(exception));
    }

    [Theory]
    [InlineData(unchecked((int)0x80070005))]
    [InlineData(unchecked((int)0x80004005))]
    [InlineData(0x00000000)]
    public void IsKnownWinUiFrameworkException_COMException_OtherHResult_ReturnsFalse(int hResult)
    {
        COMException exception = new("Other COM error", hResult);
        Assert.False(ExceptionFilter.IsKnownWinUiFrameworkException(exception));
    }

    [Fact]
    public void IsKnownWinUiFrameworkException_FileNotFoundException_NullStackTrace_ReturnsTrue()
    {
        FileNotFoundException exception = new("File not found");
        Assert.Null(exception.StackTrace);
        Assert.True(ExceptionFilter.IsKnownWinUiFrameworkException(exception));
    }

    [Fact]
    public void IsKnownWinUiFrameworkException_FileNotFoundException_WithStackTrace_ReturnsFalse()
    {
        FileNotFoundException? exception = null;
        try
        {
            throw new FileNotFoundException("File not found");
        }
        catch (FileNotFoundException ex)
        {
            exception = ex;
        }

        Assert.NotNull(exception!.StackTrace);
        Assert.False(ExceptionFilter.IsKnownWinUiFrameworkException(exception));
    }

    [Fact]
    public void IsKnownWinUiFrameworkException_OtherException_ReturnsFalse()
    {
        Assert.False(ExceptionFilter.IsKnownWinUiFrameworkException(new InvalidOperationException()));
        Assert.False(ExceptionFilter.IsKnownWinUiFrameworkException(new Exception()));
        Assert.False(ExceptionFilter.IsKnownWinUiFrameworkException(new NullReferenceException()));
    }

    [Fact]
    public void IsAllSafeNetworkException_AllTaskCanceledException_ReturnsTrue()
    {
        AggregateException exception = new(new TaskCanceledException(), new TaskCanceledException());
        Assert.True(ExceptionFilter.IsAllSafeNetworkException(exception));
    }

    [Fact]
    public void IsAllSafeNetworkException_AllHttpRequestException_ReturnsTrue()
    {
        AggregateException exception = new(new HttpRequestException(), new HttpRequestException());
        Assert.True(ExceptionFilter.IsAllSafeNetworkException(exception));
    }

    [Fact]
    public void IsAllSafeNetworkException_AllOperationCanceledException_ReturnsTrue()
    {
        AggregateException exception = new(new OperationCanceledException(), new OperationCanceledException());
        Assert.True(ExceptionFilter.IsAllSafeNetworkException(exception));
    }

    [Fact]
    public void IsAllSafeNetworkException_MixedSafeExceptions_ReturnsTrue()
    {
        AggregateException exception = new(
            new TaskCanceledException(),
            new HttpRequestException(),
            new OperationCanceledException());
        Assert.True(ExceptionFilter.IsAllSafeNetworkException(exception));
    }

    [Fact]
    public void IsAllSafeNetworkException_ContainsNonSafeException_ReturnsFalse()
    {
        AggregateException exception = new(
            new TaskCanceledException(),
            new InvalidOperationException("Not a network error"));
        Assert.False(ExceptionFilter.IsAllSafeNetworkException(exception));
    }

    [Fact]
    public void IsAllSafeNetworkException_SingleNonSafeException_ReturnsFalse()
    {
        AggregateException exception = new(new NullReferenceException());
        Assert.False(ExceptionFilter.IsAllSafeNetworkException(exception));
    }

    [Fact]
    public void IsAllSafeNetworkException_NestedAggregateException_FlattensAndChecks()
    {
        AggregateException inner = new(new TaskCanceledException());
        AggregateException outer = new(inner);
        Assert.True(ExceptionFilter.IsAllSafeNetworkException(outer));
    }

    [Fact]
    public void IsAllSafeNetworkException_NestedWithNonSafe_ReturnsFalse()
    {
        AggregateException inner = new(new TaskCanceledException(), new NullReferenceException());
        AggregateException outer = new(inner);
        Assert.False(ExceptionFilter.IsAllSafeNetworkException(outer));
    }
}
