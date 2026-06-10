//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Service.kyxsan;
using kyxsan.ViewModel.User;
using kyxsan.Web.Endpoint.kyxsan;
using kyxsan.Web.Hoyolab;
using kyxsan.Web.Hoyolab.Takumi.GameRecord;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.kyxsan.SpiralAbyss.Post;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.kyxsan.SpiralAbyss;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanSpiralAbyssClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanSpiralAbyssClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<kyxsanResponse<bool>> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RecordCheck(uid.Value))
            .Get();

        kyxsanResponse<bool>? resp = await builder
            .SendAsync<kyxsanResponse<bool>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<RankInfo>> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RecordRank(uid.Value))
            .Get();

        kyxsanResponse<RankInfo>? resp = await builder
            .SendAsync<kyxsanResponse<RankInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<Overview>> GetOverviewAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsOverview(last))
            .Get();

        kyxsanResponse<Overview>? resp = await builder
            .SendAsync<kyxsanResponse<Overview>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsAvatarAttendanceRate(last))
            .Get();

        kyxsanResponse<List<AvatarAppearanceRank>>? resp = await builder
            .SendAsync<kyxsanResponse<List<AvatarAppearanceRank>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsAvatarUtilizationRate(last))
            .Get();

        kyxsanResponse<List<AvatarUsageRank>>? resp = await builder
            .SendAsync<kyxsanResponse<List<AvatarUsageRank>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<List<AvatarCollocation>>> GetAvatarCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsAvatarAvatarCollocation(last))
            .Get();

        kyxsanResponse<List<AvatarCollocation>>? resp = await builder
            .SendAsync<kyxsanResponse<List<AvatarCollocation>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<List<WeaponCollocation>>> GetWeaponCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsWeaponWeaponCollocation(last))
            .Get();

        kyxsanResponse<List<WeaponCollocation>>? resp = await builder
            .SendAsync<kyxsanResponse<List<WeaponCollocation>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsAvatarHoldingRate(last))
            .Get();

        kyxsanResponse<List<AvatarConstellationInfo>>? resp = await builder
            .SendAsync<kyxsanResponse<List<AvatarConstellationInfo>>>(httpClient, false, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<kyxsanResponse<List<TeamAppearance>>> GetTeamCombinationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().StatisticsTeamCombination(last))
            .Get();

        kyxsanResponse<List<TeamAppearance>>? resp = await builder
            .SendAsync<kyxsanResponse<List<TeamAppearance>>>(httpClient, false, token)
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
            kyxsanUserOptions options = serviceProvider.GetRequiredService<kyxsanUserOptions>();
            string? userName = await options.GetActualUserNameAsync().ConfigureAwait(false);
            return new(userAndUid.Uid.Value, detailsWrapper.List, spiralAbyss, userName);
        }

        return default;
    }

    public async ValueTask<kyxsanResponse> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RecordUpload())
            .PostJson(playerRecord);

        kyxsanResponse? resp = await builder
            .SendAsync<kyxsanResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}