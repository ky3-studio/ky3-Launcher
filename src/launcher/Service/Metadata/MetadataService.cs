//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using kyxsan.Core.ExceptionService;
using kyxsan.Service.BackgroundActivity;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;

namespace kyxsan.Service.Metadata;

[Service(ServiceLifetime.Singleton, typeof(IMetadataService))]
internal sealed partial class MetadataService : IMetadataService
{
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly ILogger<MetadataService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly BackgroundActivityOptions backgroundActivityOptions;
    private readonly ITaskContext taskContext;
    private readonly JsonSerializerOptions options;

    private volatile bool isInitialized;

    [GeneratedConstructor]
    public partial MetadataService(IServiceProvider serviceProvider);

    public partial IMemoryCache MemoryCache { get; }

    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    public ValueTask InitializeInternalAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return ValueTask.CompletedTask;
        }

        
        isInitialized = true;
        initializeCompletionSource.TrySetResult();
        backgroundActivityOptions.MetadataInitialization.Update(taskContext, SH.ServiceMetadataInitReady, true, false, false, false);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<ImmutableArray<T>> FromCacheOrFileAsync<T>(MetadataFileStrategy strategy, CancellationToken token)
        where T : class
    {
        Verify.Operation(isInitialized, SH.ServiceMetadataNotInitialized);
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}";

        if (MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return Unsafe.Unbox<ImmutableArray<T>>(value);
        }

        return strategy.IsScattered
            ? await FromBundledScatteredFileAsync<T>(strategy, cacheKey, token).ConfigureAwait(false)
            : await FromBundledSingleFileAsync<T>(strategy, cacheKey, token).ConfigureAwait(false);
    }

    private async ValueTask<ImmutableArray<T>> FromBundledSingleFileAsync<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string fileName = $"{strategy.Name}.json";

        using (Stream? bundledStream = metadataOptions.TryGetBundledResourceStream(fileName))
        {
            if (bundledStream is null)
            {
                FileNotFoundException exception = new(SH.ServiceMetadataFileNotFound, strategy.Name);
                throw kyxsanException.Throw(SH.ServiceMetadataFileNotFound, exception);
            }

            try
            {
                ImmutableArray<T> result = await JsonSerializer.DeserializeAsync<ImmutableArray<T>>(bundledStream, options, token).ConfigureAwait(false);
                return MemoryCache.Set(cacheKey, result);
            }
            catch (Exception ex)
            {
                ex.Data.Add("FileName", strategy.Name);
                throw new InvalidDataException($"Bundled metadata corrupted: {strategy.Name}", ex);
            }
        }
    }

    private ValueTask<ImmutableArray<T>> FromBundledScatteredFileAsync<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        _ = token;

        (string FileName, byte[] Data)[] bundledResources = metadataOptions.GetBundledScatteredResources(strategy.Name);
        if (bundledResources.Length <= 0)
        {
            DirectoryNotFoundException exception = new(SH.ServiceMetadataFileNotFound);
            throw kyxsanException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        ImmutableArray<T>.Builder results = ImmutableArray.CreateBuilder<T>(bundledResources.Length);
        foreach ((string name, byte[] data) in bundledResources)
        {
            try
            {
                T? result = JsonSerializer.Deserialize<T>((ReadOnlySpan<byte>)data, options);
                ArgumentNullException.ThrowIfNull(result);
                results.Add(result);
            }
            catch (Exception ex)
            {
                ex.Data.Add("FileName", $"{strategy.Name}/{name}");
                throw new InvalidDataException($"Bundled metadata corrupted: {strategy.Name}/{name}", ex);
            }
        }

        return ValueTask.FromResult(MemoryCache.Set(cacheKey, results.ToImmutable()));
    }
}
