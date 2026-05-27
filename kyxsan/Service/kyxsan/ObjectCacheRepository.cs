//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Abstraction;

namespace kyxsan.Service.kyxsan;

[Service(ServiceLifetime.Singleton, typeof(IObjectCacheRepository))]
internal sealed partial class ObjectCacheRepository : IObjectCacheRepository
{
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial ObjectCacheRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public async ValueTask AddObjectCacheAsync<T>(string key, TimeSpan expire, T data)
        where T : class
    {
        await taskContext.SwitchToBackgroundAsync();
        this.Add(new()
        {
            Key = key,
            ExpireTime = DateTimeOffset.Now.Add(expire),
            Value = JsonSerializer.Serialize(data, jsonSerializerOptions),
        });
    }

    public async ValueTask<T?> GetObjectOrDefaultAsync<T>(string key)
        where T : class
    {
        await taskContext.SwitchToBackgroundAsync();
        if (this.SingleOrDefault(e => e.Key == key) is { } entry)
        {
            if (!entry.IsExpired)
            {
                ArgumentNullException.ThrowIfNull(entry.Value);
                return JsonSerializer.Deserialize<T>(entry.Value, jsonSerializerOptions);
            }

            this.Delete(entry);
        }

        return default;
    }
}