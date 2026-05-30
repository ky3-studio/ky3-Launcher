//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using kyxsan.Web.Hoyolab;
using kyxsan.Web.Hoyolab.Hk4e.Common.Announcement;
using kyxsan.Web.Response;
using System.Collections.Immutable;
using System.Text;
using WebAnnouncement = kyxsan.Web.Hoyolab.Hk4e.Common.Announcement.Announcement;

namespace kyxsan.Service.Announcement;

[Service(ServiceLifetime.Scoped, typeof(IAnnouncementService))]
internal sealed partial class AnnouncementService : IAnnouncementService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;
    private readonly IMemoryCache memoryCache;

    [GeneratedConstructor]
    public partial AnnouncementService(IServiceProvider serviceProvider);

    [SuppressMessage("", "SH003")]
    public async ValueTask<AnnouncementWrapper?> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken token = default)
    {
        AnnouncementWrapper? wrapper = await memoryCache.GetOrCreateAsync($"{nameof(AnnouncementService)}.{nameof(AnnouncementWrapper)}.{languageCode}.{region}", entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(30L));
            return PrivateGetAnnouncementWrapperAsync(languageCode, region, token);
        }).ConfigureAwait(false);

        return wrapper;
    }

    private static void PreprocessAnnouncements(Dictionary<int, string> contentMap, ImmutableArray<AnnouncementListWrapper> announcementListWrappers)
    {
        string? fallbackBanner = null;

        foreach (AnnouncementListWrapper listWrapper in announcementListWrappers)
        {
            foreach (WebAnnouncement item in listWrapper.List)
            {
                item.Subtitle = new StringBuilder(item.Subtitle)
                    .Replace("\r<br>", string.Empty)
                    .Replace("<br />", string.Empty)
                    .ToString();

                item.Content = AnnouncementRegex
                    .XmlTimeTagRegex
                    .Replace(contentMap.GetValueOrDefault(item.AnnId, string.Empty), x => x.Groups[1].Value);

                if (!string.IsNullOrEmpty(item.Banner))
                {
                    fallbackBanner ??= item.Banner;
                }
            }
        }

        if (fallbackBanner is not null)
        {
            foreach (AnnouncementListWrapper listWrapper in announcementListWrappers)
            {
                foreach (WebAnnouncement item in listWrapper.List)
                {
                    if (string.IsNullOrEmpty(item.Banner))
                    {
                        item.Banner = fallbackBanner;
                    }
                }
            }
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task<AnnouncementWrapper?> PrivateGetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken cancellationToken = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        ImmutableArray<AnnouncementContent> contents;
        AnnouncementWrapper? wrapper;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            AnnouncementClient announcementClient = scope.ServiceProvider.GetRequiredService<AnnouncementClient>();

            Response<AnnouncementWrapper> announcementWrapperResponse = await announcementClient
                .GetAnnouncementsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(announcementWrapperResponse, scope.ServiceProvider, out wrapper))
            {
                return default;
            }

            Response<ListWrapper<AnnouncementContent>> announcementContentResponse = await announcementClient
                .GetAnnouncementContentsAsync(languageCode, region, cancellationToken)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(announcementContentResponse, scope.ServiceProvider, out ListWrapper<AnnouncementContent>? contentsWrapper))
            {
                return default;
            }

            contents = contentsWrapper.List;
        }

        Dictionary<int, string> contentMap = contents.ToDictionaryIgnoringDuplicateKeys(content => content.AnnId, content => content.Content);

        PreprocessAnnouncements(contentMap, wrapper.List);
        return wrapper;
    }
}