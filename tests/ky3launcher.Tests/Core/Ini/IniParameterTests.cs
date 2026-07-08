using Launcher.Core.IO.Ini;
using Xunit;

namespace ky3launcher.Tests.Core.Ini;

public class IniParameterTests
{
    [Fact]
    public void ToString_FormatsKeyEqualsValue()
    {
        IniParameter param = new("key", "value");
        Assert.Equal("key=value", param.ToString());
    }

    [Fact]
    public void WithValue_SameValue_ReturnsSameInstance_ChangedFalse()
    {
        IniParameter param = new("key", "value");
        IniParameter result = param.WithValue("value", out bool changed);
        Assert.False(changed);
        Assert.Same(param, result);
    }

    [Fact]
    public void WithValue_DifferentValue_ReturnsNewInstance_ChangedTrue()
    {
        IniParameter param = new("key", "old");
        IniParameter result = param.WithValue("new", out bool changed);
        Assert.True(changed);
        Assert.NotSame(param, result);
        Assert.Equal("key", result.Key);
        Assert.Equal("new", result.Value);
    }

    [Fact]
    public void Equals_SameKeyAndValue_ReturnsTrue()
    {
        IniParameter a = new("key", "value");
        IniParameter b = new("key", "value");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentKey_ReturnsFalse()
    {
        IniParameter a = new("key1", "value");
        IniParameter b = new("key2", "value");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        IniParameter a = new("key", "value1");
        IniParameter b = new("key", "value2");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        IniParameter a = new("key", "value");
        Assert.False(a.Equals(null));
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        IniParameter a = new("key", "value");
        Assert.True(a.Equals(a));
    }

    [Fact]
    public void GetHashCode_SameKeyAndValue_SameHash()
    {
        IniParameter a = new("key", "value");
        IniParameter b = new("key", "value");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
