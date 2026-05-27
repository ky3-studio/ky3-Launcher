//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Scheme;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using static kyxsan.Service.Game.GameConstants;

namespace kyxsan.Service.Game.Package;

internal sealed class PackageConverterContext
{
    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public PackageConverterContext(LaunchScheme currentScheme, LaunchScheme targetScheme, IGameFileSystem fileSystem)
    {
        CurrentScheme = currentScheme;
        TargetScheme = targetScheme;
        GameFileSystem = fileSystem;

        ParallelOptions = new() { MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount, 16), };

        ServerCacheFolder = kyxsanRuntime.GetDataServerCacheDirectory();
        ServerCacheChunksFolder = Path.Combine(ServerCacheFolder, "Chunks");

        string serverCacheOversea = Path.Combine(ServerCacheFolder, "Oversea");
        string serverCacheChinese = Path.Combine(ServerCacheFolder, "Chinese");

        (ServerCacheBackupFolder, ServerCacheTargetFolder) = targetScheme.IsOversea
            ? (serverCacheChinese, serverCacheOversea)
            : (serverCacheOversea, serverCacheChinese);

        (FromDataFolderName, ToDataFolderName) = targetScheme.IsOversea
            ? (YuanShenData, GenshinImpactData)
            : (GenshinImpactData, YuanShenData);

        FromDataFolder = Path.Combine(fileSystem.GameDirectory, FromDataFolderName);
        ToDataFolder = Path.Combine(fileSystem.GameDirectory, ToDataFolderName);
    }

    public ParallelOptions ParallelOptions { get; }

    public string ServerCacheFolder { get; }

    public string ServerCacheChunksFolder { get; }

    public ConcurrentDictionary<string, Void> DuplicatedChunkNames { get; } = [];

    public string ServerCacheBackupFolder { get; }

    public string ServerCacheTargetFolder { get; }

    public string FromDataFolderName { get; }

    public string ToDataFolderName { get; }

    public string FromDataFolder { get; }

    public string ToDataFolder { get; }

    public LaunchScheme CurrentScheme { get; }

    public LaunchScheme TargetScheme { get; }

    public IGameFileSystem GameFileSystem { get; }

    public required HttpClient HttpClient { get; init; }

    public required IProgress<PackageConvertStatus> Progress { get; init; }

    public BranchWrapper? CurrentBranch { get; init; }

    public BranchWrapper? TargetBranch { get; init; }

    public string GetServerCacheBackupFilePath(string filePath)
    {
        return Path.Combine(ServerCacheBackupFolder, filePath);
    }

    public string GetServerCacheTargetFilePath(string filePath)
    {
        return Path.Combine(ServerCacheTargetFolder, filePath);
    }

    public string GetGameFolderFilePath(string filePath)
    {
        return Path.Combine(GameFileSystem.GameDirectory, filePath);
    }

    [SuppressMessage("", "SH003")]
    public Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token = default)
    {
        return chunkLocks.LockAsync(chunkName);
    }
}