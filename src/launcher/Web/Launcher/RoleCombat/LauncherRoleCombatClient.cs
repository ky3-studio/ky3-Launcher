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
using Launcher.ViewModel.User;
using Launcher.Web.Endpoint.Launcher;
using Launcher.Web.Hoyolab.Takumi.GameRecord;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Launcher.RoleCombat.Post;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using Launcher.Web.Response;
using System.Net.Http;

namespace Launcher.Web.Launcher.RoleCombat;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class LauncherRoleCombatClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial LauncherRoleCombatClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<LauncherResponse<RoleCombatStatisticsItem>> GetStatisticsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().RoleCombatStatistics(last))
            .Get();

        LauncherResponse<RoleCombatStatisticsItem>? resp = await builder
            .SendAsync<LauncherResponse<RoleCombatStatisticsItem>>(httpClient, token)
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

    public async ValueTask<LauncherResponse> UploadRecordAsync(SimpleRoleCombatRecord record, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().RoleCombatRecordUpload())
            .PostJson(record);

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}