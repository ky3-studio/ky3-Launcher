//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Annotation.HttpClient;
using Launcher.Web.Endpoint.Launcher;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Launcher.Web.Launcher;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class LauncherPassportClient
{
    private const string PublicKey = """
        -----BEGIN PUBLIC KEY-----
        MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAy4i0acb1/rTjwNt4Wsi/
        AgRLztRRGhludiYOWskqS6o0wTy6+DkGNZew/8qy93Tmv/mhYMoBhDJACD7Dpzu6
        2cdiPl6MW8wuAE+H86Mh7ghWxnUAvdK6Cp3qbk7MaJF2zI/2yYesGYhcf7HZmZmC
        RldyQnH9SP30FRhmbSAqGAjVSObwfa3W9islkbYB2SkcXguK+hONZmtqISoiUK1/
        d+ZEpL01MNWI06iUxin+iT3yk68o6reLOk/Yoqjj12pONIwbu7Up4noLhhhmBdR3
        7Xy9csCounngKoBw+7tEmmzJzeqYm/zeHp/Jpy/996dIxAiq6jVvNpWaT9Es6s08
        cwIDAQAB
        -----END PUBLIC KEY-----
        """;

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial LauncherPassportClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<LauncherResponse> RequestVerifyAsync(string email, VerifyCodeRequestType requestType, CancellationToken token = default)
    {
        Dictionary<string, object> data = new()
        {
            ["UserName"] = Encrypt(email),
        };

        if (requestType.HasFlag(VerifyCodeRequestType.ResetPassword))
        {
            data["IsResetPassword"] = true;
        }

        if (requestType.HasFlag(VerifyCodeRequestType.CancelRegistration))
        {
            data["IsCancelRegistration"] = true;
        }

        if (requestType.HasFlag(VerifyCodeRequestType.ResetUserName))
        {
            data["IsResetUserName"] = true;
        }

        if (requestType.HasFlag(VerifyCodeRequestType.ResetUserNameNew))
        {
            data["IsResetUserNameNew"] = true;
        }

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportVerify())
            .PostJson(data);

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<TokenSet>> RegisterAsync(string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportRegister())
            .PostJson(data);

        LauncherResponse<TokenSet>? resp = await builder
            .SendAsync<LauncherResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse> UnregisterAsync(string? accessToken, string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportCancel())
            .SetAccessToken(accessToken)
            .PostJson(data);

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<TokenSet>> ResetUserNameAsync(string email, string newUserName, string verifyCode, string newVerifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["NewUserName"] = Encrypt(newUserName),
            ["VerifyCode"] = Encrypt(verifyCode),
            ["NewVerifyCode"] = Encrypt(newVerifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportResetUserName())
            .PostJson(data);

        LauncherResponse<TokenSet>? resp = await builder
            .SendAsync<LauncherResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<TokenSet>> ResetPasswordAsync(string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportResetPassword())
            .PostJson(data);

        LauncherResponse<TokenSet>? resp = await builder
            .SendAsync<LauncherResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<TokenSet>> LoginAsync(string email, string password, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportLogin())
            .PostJson(data);

        LauncherResponse<TokenSet>? resp = await builder
            .SendAsync<LauncherResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<UserInfo>> GetUserInfoAsync(string? accessToken, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportUserInfo())
            .SetAccessToken(accessToken)
            .Get();

        LauncherResponse<UserInfo>? resp = await builder
            .SendAsync<LauncherResponse<UserInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse<TokenSet>> RefreshTokenAsync(string refreshToken, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["RefreshToken"] = Encrypt(refreshToken),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportRefreshToken())
            .PostJson(data);

        LauncherResponse<TokenSet>? resp = await builder
            .SendAsync<LauncherResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse> RevokeTokenAsync(string? accessToken, string deviceId, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["DeviceId"] = Encrypt(deviceId),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportRevokeToken())
            .SetAccessToken(accessToken)
            .PostJson(data);

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<LauncherResponse> RevokeAllTokensAsync(string? accessToken, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(LauncherEndpointsFactory.Create().PassportRevokeAllTokens())
            .SetAccessToken(accessToken)
            .Post();

        LauncherResponse? resp = await builder
            .SendAsync<LauncherResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    private static string Encrypt(string text)
    {
        using (RSA rsa = RSA.Create(2048))
        {
            rsa.ImportFromPem(PublicKey);
            byte[] encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.OaepSHA1);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}