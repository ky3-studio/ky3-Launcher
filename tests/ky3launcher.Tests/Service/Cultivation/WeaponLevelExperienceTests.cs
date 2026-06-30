using Launcher.Model.Intrinsic;
using Launcher.Service.Cultivation.Offline;
using Xunit;

namespace ky3launcher.Tests.Service.Cultivation;

public class WeaponLevelExperienceTests
{
    [Theory]
    [InlineData(QualityType.QUALITY_ORANGE, 1, 2, 600)]
    [InlineData(QualityType.QUALITY_PURPLE, 1, 2, 400)]
    [InlineData(QualityType.QUALITY_BLUE, 1, 2, 275)]
    [InlineData(QualityType.QUALITY_GREEN, 1, 2, 175)]
    [InlineData(QualityType.QUALITY_WHITE, 1, 2, 125)]
    public void Level1To2_ReturnsCorrectBaseExp(QualityType quality, int from, int to, int expected)
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(quality, from, to);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FiveStar_Level1To90_ReturnsPositiveValue()
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 1, 90);
        Assert.True(result > 0);
    }

    [Fact]
    public void FiveStar_Level1To90_IsGreaterThanFourStar()
    {
        int fiveStar = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 1, 90);
        int fourStar = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_PURPLE, 1, 90);
        Assert.True(fiveStar > fourStar);
    }

    [Theory]
    [InlineData(QualityType.QUALITY_ORANGE)]
    [InlineData(QualityType.QUALITY_PURPLE)]
    [InlineData(QualityType.QUALITY_BLUE)]
    public void ThreeStarAndAbove_Target91_ReturnsZero(QualityType quality)
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(quality, 1, 91);
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData(QualityType.QUALITY_GREEN)]
    [InlineData(QualityType.QUALITY_WHITE)]
    public void TwoStarAndBelow_Target71_ReturnsZero(QualityType quality)
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(quality, 1, 71);
        Assert.Equal(0, result);
    }

    [Fact]
    public void GreenWeapon_Level1To70_ReturnsPositiveValue()
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_GREEN, 1, 70);
        Assert.True(result > 0);
    }

    [Fact]
    public void SameLevel_ReturnsZero()
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 50, 50);
        Assert.Equal(0, result);
    }

    [Fact]
    public void ReversedLevels_ReturnsZero()
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 80, 50);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CurrentLevelBelowOne_ReturnsZero()
    {
        int result = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 0, 50);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Additivity_FiveStar_Level1To50EqualsParts()
    {
        int full = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 1, 50);
        int part1 = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 1, 30);
        int part2 = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 30, 50);
        Assert.Equal(full, part1 + part2);
    }

    [Fact]
    public void HigherQuality_HigherExpRequirement()
    {
        int orange = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_ORANGE, 1, 50);
        int purple = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_PURPLE, 1, 50);
        int blue = WeaponLevelExperience.CalculateTotalExperience(QualityType.QUALITY_BLUE, 1, 50);

        Assert.True(orange > purple);
        Assert.True(purple > blue);
    }
}
