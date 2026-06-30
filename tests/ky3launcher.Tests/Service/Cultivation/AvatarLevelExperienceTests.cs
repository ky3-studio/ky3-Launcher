using Launcher.Service.Cultivation.Offline;
using Xunit;

namespace ky3launcher.Tests.Service.Cultivation;

public class AvatarLevelExperienceTests
{
    [Fact]
    public void Level1To2_Returns1000()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(1, 2);
        Assert.Equal(1000, result);
    }

    [Fact]
    public void Level1To10_ReturnsSumOfFirst9Levels()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(1, 10);
        int expected = 1000 + 1325 + 1700 + 2150 + 2625 + 3150 + 3725 + 4350 + 5000;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Level1To90_ReturnsPositiveValue()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(1, 90);
        Assert.True(result > 0);
    }

    [Fact]
    public void SameLevel_ReturnsZero()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(50, 50);
        Assert.Equal(0, result);
    }

    [Fact]
    public void ReversedLevels_ReturnsZero()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(80, 50);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TargetExceeds90_ReturnsZero()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(1, 91);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CurrentLevelBelowOne_ReturnsZero()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(0, 50);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Additivity_Level1To50EqualsLevel1To30PlusLevel30To50()
    {
        int full = AvatarLevelExperience.CalculateTotalExperience(1, 50);
        int part1 = AvatarLevelExperience.CalculateTotalExperience(1, 30);
        int part2 = AvatarLevelExperience.CalculateTotalExperience(30, 50);
        Assert.Equal(full, part1 + part2);
    }

    [Fact]
    public void Level80To90_ReturnsKnownValue()
    {
        int result = AvatarLevelExperience.CalculateTotalExperience(80, 90);
        int expected = 183175 + 216225 + 243025 + 273100 + 306800 + 344600 + 386950 + 434425 + 487625 + 547200;
        Assert.Equal(expected, result);
    }
}
