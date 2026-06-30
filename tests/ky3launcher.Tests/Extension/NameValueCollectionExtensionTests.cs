using System.Collections.Specialized;
using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class NameValueCollectionExtensionTests
{
    [Fact]
    public void TryGetSingleValue_SingleValue_ReturnsTrueAndValue()
    {
        NameValueCollection nvc = new() { { "key", "value" } };
        bool success = nvc.TryGetSingleValue("key", out string? value);
        Assert.True(success);
        Assert.Equal("value", value);
    }

    [Fact]
    public void TryGetSingleValue_KeyNotExists_ReturnsFalse()
    {
        NameValueCollection nvc = new() { { "key", "value" } };
        bool success = nvc.TryGetSingleValue("missing", out string? value);
        Assert.False(success);
        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void TryGetSingleValue_MultipleValues_ReturnsFalse()
    {
        NameValueCollection nvc = new();
        nvc.Add("key", "val1");
        nvc.Add("key", "val2");
        bool success = nvc.TryGetSingleValue("key", out _);
        Assert.False(success);
    }

    [Fact]
    public void TryGetSingleValue_EmptyCollection_ReturnsFalse()
    {
        NameValueCollection nvc = new();
        bool success = nvc.TryGetSingleValue("any", out _);
        Assert.False(success);
    }
}
