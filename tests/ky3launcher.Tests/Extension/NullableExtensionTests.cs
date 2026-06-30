using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class NullableExtensionTests
{
    [Fact]
    public void TryGetValue_HasValue_ReturnsTrueAndValue()
    {
        int? nullable = 42;
        bool success = nullable.TryGetValue(out int value);
        Assert.True(success);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGetValue_Null_ReturnsFalseAndDefault()
    {
        int? nullable = null;
        bool success = nullable.TryGetValue(out int value);
        Assert.False(success);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryGetValue_StructType_Works()
    {
        DateTime? nullable = new DateTime(2024, 1, 1);
        bool success = nullable.TryGetValue(out DateTime value);
        Assert.True(success);
        Assert.Equal(new DateTime(2024, 1, 1), value);
    }
}
