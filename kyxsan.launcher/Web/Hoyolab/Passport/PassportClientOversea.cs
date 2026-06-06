//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Annotation.HttpClient;
using kyxsan.Model.Entity;
using kyxsan.Web.Endpoint.Hoyolab;
using kyxsan.Web.Hoyolab.DataSigning;
using kyxsan.Web.Request.Builder;
using kyxsan.Web.Request.Builder.Abstraction;
using kyxsan.Web.Response;
using System.Net.Http;
using System.Net.Http.Headers;

namespace kyxsan.Web.Hoyolab.Passport;

[HttpClient(HttpClientConfiguration.XRpc3)]
internal sealed partial class PassportClientOversea : IPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial PassportClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        Response<STokenV2TokensWrapper> response = await GetTokensBySTokenV2Async(user, [4], token).ConfigureAwait(false);

        if (response.ReturnCode is 0 && response.Data?.Tokens is { } tokens)
        {
            TokenWrapper? target = tokens.FirstOrDefault(t => t.TokenType is 4);
            if (target is not null)
            {
                UidCookieToken result = new() { Uid = user.Aid, CookieToken = target.Token };
                return new(0, response.Message, result);
            }
        }

        return Response.Response.CloneReturnCodeAndMessage<UidCookieToken, STokenV2TokensWrapper>(response);
    }

    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        Response<STokenV2TokensWrapper> response = await GetTokensBySTokenV2Async(user, [2], token).ConfigureAwait(false);

        if (response.ReturnCode is 0 && response.Data?.Tokens is { } tokens)
        {
            TokenWrapper? target = tokens.FirstOrDefault(t => t.TokenType is 2);
            if (target is not null)
            {
                LTokenWrapper result = new() { LToken = target.Token };
                return new(0, response.Message, result);
            }
        }

        return Response.Response.CloneReturnCodeAndMessage<LTokenWrapper, STokenV2TokensWrapper>(response);
    }

    private async ValueTask<Response<STokenV2TokensWrapper>> GetTokensBySTokenV2Async(User user, int[] tokenTypes, CancellationToken token)
    {
        string stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN) ?? string.Empty;
        string mid = user.Mid ?? string.Empty;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetTokenBySTokenV2())
            .SetHeader("Cookie", $"stoken_v2={stoken};mid={mid}")
            .SetHeader("x-rpc-app_id", "c9oqaq3s3gu8");

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.OSAppLogin, false).ConfigureAwait(false);

        builder.PostJson(new Dictionary<string, object> { ["dst_token_types"] = tokenTypes });

        Response<STokenV2TokensWrapper>? resp = await builder
            .SendAsync<Response<STokenV2TokensWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public ValueTask<Response<UserInfoWrapper>> VerifyLtokenAsync(User user, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<UserInfoWrapper>>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginBySTokenAsync(Cookie stokenV1, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginByGameTokenAsync(UidGameToken account, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public ValueTask<(string? Aigis, Response<MobileCaptcha> Response)> CreateLoginCaptchaAsync(string mobile, string? aigis, CancellationToken token = default)
    {
        return ValueTask.FromException<(string? Aigis, Response<MobileCaptcha> Response)>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(IPassportMobileCaptchaProvider provider, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(string actionType, string mobile, string captcha, string? aigis, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public async ValueTask<Response<ActionTicketInfo>> GetActionTicketInfoAsync(string ticket, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ticket);

        ActionTicketInfoRequestOversea data = new()
        {
            ActionTicket = ticket,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetActionTicketInfo())
            .PostJson(data);

        Response<ActionTicketInfo>? resp = await builder
            .SendAsync<Response<ActionTicketInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response.Response> VerifyActionTicketPartlyAsync(string ticket, string captcha, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ticket);
        ArgumentException.ThrowIfNullOrEmpty(captcha);

        ActionTicketInfoRequestOversea data = new()
        {
            ActionTicket = ticket,
            EmailCaptcha = captcha,
            VerifyMethod = 2,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountVerifyActionTicketPartly())
            .PostJson(data);

        Response.Response? resp = await builder
            .SendAsync<Response.Response>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<(string? Aigis, Response.Response Response)> CreateEmailCaptchaByActionTicketAsync(string ticket, string? aigis, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ticket);

        ActionTicketInfoRequestOversea data = new()
        {
            ActionTicket = ticket,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateEmailCaptchaByActionTicket())
            .PostJson(data);

        if (!string.IsNullOrEmpty(aigis))
        {
            builder.SetXrpcAigis(aigis);
        }

        (HttpResponseHeaders? headers, Response.Response? resp) = await builder
            .SendAsync<Response.Response>(httpClient, token)
            .ConfigureAwait(false);

        string? rpcAigis = headers?.GetValuesOrDefault("X-Rpc-Aigis")?.SingleOrDefault();
        return (rpcAigis, Response.Response.DefaultIfNull(resp));
    }
}