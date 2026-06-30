using Launcher.Extension;
using Xunit;

namespace ky3launcher.Tests.Extension;

public class NumberExtensionTests
{
    [Theory]
    [InlineData(1, "I")]
    [InlineData(4, "IV")]
    [InlineData(5, "V")]
    [InlineData(9, "IX")]
    [InlineData(10, "X")]
    [InlineData(40, "XL")]
    [InlineData(50, "L")]
    [InlineData(90, "XC")]
    [InlineData(100, "C")]
    [InlineData(400, "CD")]
    [InlineData(500, "D")]
    [InlineData(900, "CM")]
    [InlineData(1000, "M")]
    [InlineData(1994, "MCMXCIV")]
    [InlineData(2024, "MMXXIV")]
    [InlineData(3999, "MMMCMXCIX")]
    [InlineData(58, "LVIII")]
    [InlineData(1776, "MDCCLXXVI")]
    public void ToRoman_KnownValues_ReturnsCorrectResult(int input, string expected)
    {
        string result = input.ToRoman();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4000)]
    [InlineData(int.MaxValue)]
    public void ToRoman_OutOfRange_ThrowsArgumentOutOfRangeException(int input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => input.ToRoman());
    }

    [Fact]
    public void ToRoman_AllSingleDigits_AreValid()
    {
        for (int i = 1; i <= 9; i++)
        {
            string result = i.ToRoman();
            Assert.False(string.IsNullOrEmpty(result));
        }
    }

    [Theory]
    [InlineData(1u, 1u)]
    [InlineData(9u, 1u)]
    [InlineData(10u, 2u)]
    [InlineData(99u, 2u)]
    [InlineData(100u, 3u)]
    [InlineData(999u, 3u)]
    [InlineData(1000u, 4u)]
    [InlineData(12345u, 5u)]
    [InlineData(999999u, 6u)]
    public void StringLength_ReturnsDigitCount(uint input, uint expected)
    {
        Assert.Equal(expected, input.StringLength);
    }
}
