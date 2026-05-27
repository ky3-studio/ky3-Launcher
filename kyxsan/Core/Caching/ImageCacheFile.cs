//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using kyxsan.Core.IO;
using kyxsan.Core.IO.Hashing;
using System.IO;
using System.Security.Cryptography;

namespace kyxsan.Core.Caching;

internal sealed class ImageCacheFile
{
    private readonly string directory;

    private ImageCacheFile(ValueDirectory directory, string hashedFileName)
    {
        this.directory = directory;
        HashedFileName = hashedFileName;
    }

    public string HashedFileName { get; }

    [field: MaybeNull]
    public string DefaultFilePath
    {
        get => field ??= Path.GetFullPath(Path.Combine(directory, HashedFileName));
    }

    public static ImageCacheFile Create(ValueDirectory folder, string url)
    {
        return new(folder, GetHashedFileName(url));
    }

    public static ImageCacheFile Create(ValueDirectory folder, Uri uri)
    {
        return new(folder, GetHashedFileName(uri.OriginalString));
    }

    public static ValueFile GetHashedFile(ValueDirectory folder, string url)
    {
        return Path.GetFullPath(Path.Combine(folder, GetHashedFileName(url)));
    }

    public static ValueFile GetHashedFile(ValueDirectory folder, Uri uri)
    {
        return Path.GetFullPath(Path.Combine(folder, GetHashedFileName(uri.OriginalString)));
    }

    public static string GetHashedFileName(string url)
    {
        return Hash.ToHexString(HashAlgorithmName.SHA1, url);
    }

    public ValueFile GetThemedFile(ElementTheme theme)
    {
        return theme is ElementTheme.Default
            ? DefaultFilePath
            : Path.GetFullPath(Path.Combine(directory, $"{theme}", HashedFileName));
    }
}