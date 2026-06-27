//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Launcher.Core.Logging;
using Launcher.Service.Constants;
using Launcher.Service.Yae.Achievement;
using System.Net.Http;

namespace Launcher.Service.Yae;

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
    private readonly IHttpClientFactory httpClientFactory;

    [GeneratedConstructor]
    public partial YaeCdnClient(IServiceProvider serviceProvider);

    public async ValueTask<YaeAchievementInfo?> GetMetadataAsync(CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(CacheKey, out YaeAchievementInfo? cached))
        {
            return cached;
        }

        using HttpClient httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.ImageDownloadTimeoutSeconds);

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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                    "CDN fetch failed",
                    "YaeCdnClient",
                    [("Url", url), ("Error", ex.Message)]));
            }
        }

        return default;
    }
}
