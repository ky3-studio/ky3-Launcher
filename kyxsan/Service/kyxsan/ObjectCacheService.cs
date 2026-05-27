//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using kyxsan.Core.Abstraction;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Response;

namespace kyxsan.Service.kyxsan;

internal abstract partial class ObjectCacheService : ITypeName
{
    private static readonly TimeSpan CacheExpireTime = TimeSpan.FromHours(2);

    private readonly IObjectCacheRepository objectCacheRepository;
    private readonly IMemoryCache memoryCache;

    [GeneratedConstructor]
    public partial ObjectCacheService(IServiceProvider serviceProvider);

    public abstract string TypeName { get; }

    protected partial IServiceProvider ServiceProvider { get; }

    protected async ValueTask<T> FromCacheOrWebAsync<T>(string typeName, bool last, Func<bool, CancellationToken, ValueTask<kyxsanResponse<T>>> taskFunc)
        where T : class, new()
    {
        string key = $"{TypeName}.Cache.{typeName}.{(last ? "Last" : "Current")}";
        if (memoryCache.TryGetValue(key, out object? cache))
        {
            T? t = cache as T;
            ArgumentNullException.ThrowIfNull(t);
            return t;
        }

        if (await objectCacheRepository.GetObjectOrDefaultAsync<T>(key).ConfigureAwait(false) is { } value)
        {
            return memoryCache.Set(key, value, CacheExpireTime);
        }

        Response<T> webResponse = await taskFunc(last, default).ConfigureAwait(false);
        T? data = webResponse.Data;

        if (data is not null)
        {
            await objectCacheRepository.AddObjectCacheAsync(key, CacheExpireTime, data).ConfigureAwait(false);
        }

        return memoryCache.Set(key, data ?? new(), CacheExpireTime);
    }
}