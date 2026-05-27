//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.RateLimiting;

namespace kyxsan.Service.Game.Package.Advanced;

internal readonly struct GamePackageServiceContext
{
    public readonly GamePackageOperationContext Operation;
    public readonly GamePackageOperationInfo Information;
    public readonly IProgress<GamePackageOperationReport> Progress;
    public readonly ParallelOptions ParallelOptions;
    public readonly ConcurrentDictionary<string, Void> DuplicatedChunkNames = [];
    public readonly HttpClient HttpClient;
    public readonly TokenBucketRateLimiter? StreamCopyRateLimiter;
    public readonly ConcurrentDictionary<string, Void> DownloadedPatches = [];

    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public GamePackageServiceContext(GamePackageOperationContext operation, GamePackageOperationInfo information, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, HttpClient httpClient, TokenBucketRateLimiter? rateLimiter)
    {
        Operation = operation;
        Information = information;
        Progress = progress;
        ParallelOptions = parallelOptions;
        HttpClient = httpClient;
        StreamCopyRateLimiter = rateLimiter;
    }

    public CancellationToken CancellationToken { get => ParallelOptions.CancellationToken; }

    public string EnsureAssetTargetDirectoryExists(string assetName)
    {
        if (Operation.Kind is GamePackageOperationKind.ExtractBlocks or GamePackageOperationKind.ExtractExecutable)
        {
            assetName = Path.GetFileName(assetName);
        }

        string targetPath = Path.Combine(Operation.EffectiveGameDirectory, assetName);

        string? directory = Path.GetDirectoryName(targetPath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        return targetPath;
    }

    [SuppressMessage("", "SH003")]
    public Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token)
    {
        return chunkLocks.LockAsync(chunkName);
    }
}