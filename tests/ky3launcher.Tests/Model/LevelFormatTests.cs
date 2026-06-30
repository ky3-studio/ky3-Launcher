using Launcher.Model.Primitive;
using Xunit;

namespace ky3launcher.Tests.Model;

public class LevelFormatTests
{
    [Theory]
    [InlineData(1u, "Lv. 1")]
    [InlineData(90u, "Lv. 90")]
    [InlineData(0u, "Lv. 0")]
    public void Format_SingleValue_ReturnsExpected(uint value, string expected)
    {
        Assert.Equal(expected, LevelFormat.Format(value));
    }

    [Theory]
    [InlineData(80u, 10u, "Lv. 90 (80 +10)")]
    [InlineData(70u, 5u, "Lv. 75 (70 +5)")]
    [InlineData(50u, 0u, "Lv. 50")]
    public void Format_WithExtra_ReturnsExpected(uint value, uint extra, string expected)
    {
        Assert.Equal(expected, LevelFormat.Format(value, extra));
    }
}
