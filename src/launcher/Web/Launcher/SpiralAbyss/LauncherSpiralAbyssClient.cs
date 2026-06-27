//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Core.DependencyInjection.Annotation.HttpClient;
using Launcher.Service.Launcher;
using Launcher.ViewModel.User;
using Launcher.Web.Endpoint.Launcher;
using Launcher.Web.Hoyolab;
using Launcher.Web.Hoyolab.Takumi.GameRecord;
using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Launcher.SpiralAbyss.Post;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using Launcher.Web.Response;
using System.Net.Http;

namespace Launcher.Web.Launcher.SpiralAbyss;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class LauncherSpiralAbyssClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial LauncherSpiralAbyssClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<LauncherResponse<bool>> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().RecordCheck(uid.Value))
            .Get();

        LauncherResponse<bool>? resp = await builder
            .SendAsync<LauncherResponse<bool>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<RankInfo>> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().RecordRank(uid.Value))
            .Get();

        LauncherResponse<RankInfo>? resp = await builder
            .SendAsync<LauncherResponse<RankInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<Overview>> GetOverviewAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsOverview(last))
            .Get();

        LauncherResponse<Overview>? resp = await builder
            .SendAsync<LauncherResponse<Overview>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsAvatarAttendanceRate(last))
            .Get();

        LauncherResponse<List<AvatarAppearanceRank>>? resp = await builder
            .SendAsync<LauncherResponse<List<AvatarAppearanceRank>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsAvatarUtilizationRate(last))
            .Get();

        LauncherResponse<List<AvatarUsageRank>>? resp = await builder
            .SendAsync<LauncherResponse<List<AvatarUsageRank>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<List<AvatarCollocation>>> GetAvatarCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsAvatarAvatarCollocation(last))
            .Get();

        LauncherResponse<List<AvatarCollocation>>? resp = await builder
            .SendAsync<LauncherResponse<List<AvatarCollocation>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<List<WeaponCollocation>>> GetWeaponCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsWeaponWeaponCollocation(last))
            .Get();

        LauncherResponse<List<WeaponCollocation>>? resp = await builder
            .SendAsync<LauncherResponse<List<WeaponCollocation>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsAvatarHoldingRate(last))
            .Get();

        LauncherResponse<List<AvatarConstellationInfo>>? resp = await builder
            .SendAsync<LauncherResponse<List<AvatarConstellationInfo>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<List<TeamAppearance>>> GetTeamCombinationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().StatisticsTeamCombination(last))
            .Get();

        LauncherResponse<List<TeamAppearance>>? resp = await builder
            .SendAsync<LauncherResponse<List<TeamAppearance>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<SimpleRecord?> GetPlayerRecordAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        IGameRecordClient gameRecordClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.IsOversea);

        Response<PlayerInfo> playerInfoResponse = await gameRecordClient
            .GetPlayerInfoAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(playerInfoResponse, serviceProvider))
        {
            return default;
        }

        Response<ListWrapper<Character>> listResponse = await gameRecordClient
            .GetCharacterListAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(listResponse, serviceProvider, out ListWrapper<Character>? charactersWrapper))
        {
            return default;
        }

        Response<ListWrapper<DetailedCharacter>> detailResponse = await gameRecordClient
            .GetCharacterDetailAsync(userAndUid, charactersWrapper.List.SelectAsArray(static c => c.Id), token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(detailResponse, serviceProvider, out ListWrapper<DetailedCharacter>? detailsWrapper))
        {
            return default;
        }

        Response<Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> spiralAbyssResponse = await gameRecordClient
            .GetSpiralAbyssAsync(userAndUid, ScheduleType.Current, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(spiralAbyssResponse, serviceProvider, out Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? spiralAbyss))
        {
            LauncherUserOptions options = serviceProvider.GetRequiredService<LauncherUserOptions>();
            string? userName = await options.GetActualUserNameAsync().ConfigureAwait(false);
            return new(userAndUid.Uid.Value, detailsWrapper.List, spiralAbyss, userName);
        }

        return default;
    }

    public async ValueTask<LauncherResponse> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().RecordUpload())
            .PostJson(playerRecord);

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}