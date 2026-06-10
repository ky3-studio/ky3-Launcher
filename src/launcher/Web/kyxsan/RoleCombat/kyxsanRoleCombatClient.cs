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
using kyxsan.ViewModel.User;
using kyxsan.Web.Endpoint.kyxsan;
using kyxsan.Web.Hoyolab.Takumi.GameRecord;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.kyxsan.RoleCombat.Post;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;

namespace kyxsan.Web.kyxsan.RoleCombat;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class kyxsanRoleCombatClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial kyxsanRoleCombatClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<kyxsanResponse<RoleCombatStatisticsItem>> GetStatisticsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RoleCombatStatistics(last))
            .Get();

        kyxsanResponse<RoleCombatStatisticsItem>? resp = await builder
            .SendAsync<kyxsanResponse<RoleCombatStatisticsItem>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<SimpleRoleCombatRecord?> GetPlayerRecordAsync(UserAndUid userAndUid, CancellationToken token = default)
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

        Response<Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat> roleCombatResponse = await gameRecordClient
            .GetRoleCombatAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(roleCombatResponse, serviceProvider, out Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat? roleCombat))
        {
            return default;
        }

        if (roleCombat.Data.FirstOrDefault() is { HasData: true } data)
        {
            return new(userAndUid.Uid.Value, data.Detail.BackupAvatars.SelectAsArray(static a => a.AvatarId.Value), data.Schedule.ScheduleId.Value);
        }

        return default;
    }

    public async ValueTask<kyxsanResponse> UploadRecordAsync(SimpleRoleCombatRecord record, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(kyxsanEndpointsFactory.Create().RoleCombatRecordUpload())
            .PostJson(record);

        kyxsanResponse? resp = await builder
            .SendAsync<kyxsanResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}