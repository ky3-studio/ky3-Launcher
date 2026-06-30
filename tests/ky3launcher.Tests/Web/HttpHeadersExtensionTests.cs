using System.Net.Http.Headers;
using Launcher.Web;
using Xunit;

namespace ky3launcher.Tests.Web;

public class HttpHeadersExtensionTests
{
    [Fact]
    public void AddWithUnknownValueCount_WithValues_AddsValues()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.AddWithUnknownValueCount("X-Custom", ["val1", "val2"]);
        IEnumerable<string>? values = msg.Headers.GetValues("X-Custom");
        Assert.Contains("val1", values);
        Assert.Contains("val2", values);
    }

    [Fact]
    public void AddWithUnknownValueCount_NullValues_AddsEmptyString()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.AddWithUnknownValueCount("X-Custom", null);
        IEnumerable<string>? values = msg.Headers.GetValues("X-Custom");
        Assert.Contains(string.Empty, values);
    }

    [Fact]
    public void AddWithUnknownValueCount_AllNullValues_AddsEmptyString()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.AddWithUnknownValueCount("X-Custom", [null, null]);
        IEnumerable<string>? values = msg.Headers.GetValues("X-Custom");
        Assert.Contains(string.Empty, values);
    }

    [Fact]
    public void GetValuesOrDefault_Exists_ReturnsValues()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.Add("X-Test", "hello");
        IEnumerable<string>? values = msg.Headers.GetValuesOrDefault("X-Test");
        Assert.NotNull(values);
        Assert.Contains("hello", values);
    }

    [Fact]
    public void GetValuesOrDefault_NotExists_ReturnsNull()
    {
        using HttpRequestMessage msg = new();
        IEnumerable<string>? values = msg.Headers.GetValuesOrDefault("X-Missing");
        Assert.Null(values);
    }

    [Fact]
    public void Remove_RemovesSpecifiedHeaders()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.Add("X-A", "1");
        msg.Headers.Add("X-B", "2");
        msg.Headers.Add("X-C", "3");
        msg.Headers.Remove(["X-A", "X-C"]);
        Assert.False(msg.Headers.Contains("X-A"));
        Assert.True(msg.Headers.Contains("X-B"));
        Assert.False(msg.Headers.Contains("X-C"));
    }

    [Fact]
    public void Remove_NullInput_NoOp()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.Add("X-Keep", "value");
        msg.Headers.Remove((IEnumerable<string?>?)null);
        Assert.True(msg.Headers.Contains("X-Keep"));
    }

    [Fact]
    public void Remove_WithNullEntries_SkipsNulls()
    {
        using HttpRequestMessage msg = new();
        msg.Headers.Add("X-A", "1");
        msg.Headers.Add("X-B", "2");
        msg.Headers.Remove(["X-A", null, "X-B"]);
        Assert.False(msg.Headers.Contains("X-A"));
        Assert.False(msg.Headers.Contains("X-B"));
    }
}
