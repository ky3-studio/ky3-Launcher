//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using kyxsan.Service.Yae.Achievement;
using System.Net.Http;

namespace kyxsan.Service.Yae;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class YaeCdnClient
{
    private const string CacheKey = "YaeCdnClient.Metadata";

    private static readonly string[] CdnUrls =
    [
        "https://rin.holohat.work/schicksal/metadata",
        "https://ena-rin.holohat.work/schicksal/metadata",
        "https://cn-cd-1259389942.file.myqcloud.com/schicksal/metadata",
    ];

    private readonly IMemoryCache memoryCache;

    [GeneratedConstructor]
    public partial YaeCdnClient(IServiceProvider serviceProvider);

    public async ValueTask<YaeAchievementInfo?> GetMetadataAsync(CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(CacheKey, out YaeAchievementInfo? cached))
        {
            return cached;
        }

        using HttpClient httpClient = new();
        httpClient.Timeout = TimeSpan.FromSeconds(15);

        foreach (string url in CdnUrls)
        {
            try
            {
                byte[] data = await httpClient.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);
                YaeAchievementInfo info = YaeAchievementInfo.Parse(data);

                if (info.PbInfo is not null && info.NativeConfig is not null)
                {
                    memoryCache.Set(CacheKey, info, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromHours(6),
                    });

                    return info;
                }
            }
            catch
            {
            }
        }

        return default;
    }
}
