using Launcher.Core;
using Xunit;

namespace ky3launcher.Tests.Core;

public class UrlPathTests
{
    [Fact]
    public void GetDirectoryOrRootName_FileInDirectory_ReturnsDirectory()
    {
        string? result = UrlPath.GetDirectoryOrRootName(@"C:\Users\test\file.txt");
        Assert.Equal(@"C:\Users\test", result);
    }

    [Fact]
    public void GetDirectoryOrRootName_RootFile_ReturnsRoot()
    {
        string? result = UrlPath.GetDirectoryOrRootName(@"C:\file.txt");
        Assert.Equal(@"C:\", result);
    }

    [Fact]
    public void IsEqualOrSubdirectory_SameDirectory_ReturnsTrue()
    {
        Assert.True(UrlPath.IsEqualOrSubdirectory(@"C:\Games", @"C:\Games"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_DirectChild_ReturnsTrue()
    {
        Assert.True(UrlPath.IsEqualOrSubdirectory(@"C:\Games", @"C:\Games\Genshin"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_DeepChild_ReturnsTrue()
    {
        Assert.True(UrlPath.IsEqualOrSubdirectory(@"C:\Games", @"C:\Games\Genshin\Data\StreamingAssets"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_DifferentDirectory_ReturnsFalse()
    {
        Assert.False(UrlPath.IsEqualOrSubdirectory(@"C:\Games", @"C:\Users"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_ParentIsChild_ReturnsFalse()
    {
        Assert.False(UrlPath.IsEqualOrSubdirectory(@"C:\Games\Genshin", @"C:\Games"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_SimilarPrefix_ReturnsFalse()
    {
        Assert.False(UrlPath.IsEqualOrSubdirectory(@"C:\Games", @"C:\GamesBackup"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_TrailingSlash_ReturnsTrue()
    {
        Assert.True(UrlPath.IsEqualOrSubdirectory(@"C:\Games\", @"C:\Games\Sub"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_CaseInsensitive_ReturnsTrue()
    {
        Assert.True(UrlPath.IsEqualOrSubdirectory(@"C:\GAMES", @"C:\games\sub"));
    }

    [Fact]
    public void IsEqualOrSubdirectory_EmptyDirectory_ThrowsArgumentException()
    {
        Assert.ThrowsAny<ArgumentException>(() => UrlPath.IsEqualOrSubdirectory("", @"C:\Anything"));
    }
}
