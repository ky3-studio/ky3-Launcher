//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.IO;
using System.Reflection;

namespace kyxsan.Service.Metadata;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class MetadataOptions
{
    private static readonly Assembly ResourceAssembly = typeof(MetadataOptions).Assembly;
    private static readonly string BundledResourceRoot = Path.Combine(AppContext.BaseDirectory, "Resources", "Metadata");

    private readonly CultureOptions cultureOptions;

    [GeneratedConstructor]
    public partial MetadataOptions(IServiceProvider serviceProvider);

    // 元数据完全本地自包含：仅通过随程序发布的 Resources\Metadata\{locale}\ 读取，不访问网络、不写入用户目录。
    private string BundledResourceDirectory
    {
        get => Path.Combine(BundledResourceRoot, cultureOptions.LocaleName);
    }

    public Stream? TryGetBundledResourceStream(string fileNameWithExtension)
    {
        string path = Path.Combine(BundledResourceDirectory, fileNameWithExtension);
        if (File.Exists(path))
        {
            return File.OpenRead(path);
        }

        string baseName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
        string wrappedPath = Path.Combine(BundledResourceDirectory, baseName, fileNameWithExtension);
        if (File.Exists(wrappedPath))
        {
            return File.OpenRead(wrappedPath);
        }

        return null;
    }

    public (string FileName, byte[] Data)[] GetBundledScatteredResources(string directoryName)
    {
        string dir = Path.Combine(BundledResourceDirectory, directoryName);
        if (!Directory.Exists(dir))
        {
            return [];
        }

        List<(string, byte[])> result = [];
        foreach (string path in Directory.EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly))
        {
            try
            {
                result.Add((Path.GetFileName(path), File.ReadAllBytes(path)));
            }
            catch
            {
                continue;
            }
        }

        return [.. result];
    }
}
