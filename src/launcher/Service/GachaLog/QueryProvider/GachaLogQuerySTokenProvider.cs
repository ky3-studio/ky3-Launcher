//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.User;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using kyxsan.Web.Response;
using System.Collections.Specialized;
using System.Web;

namespace kyxsan.Service.GachaLog.QueryProvider;

[Service(ServiceLifetime.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOptionKind.SToken)]
internal sealed partial class GachaLogQuerySTokenProvider : IGachaLogQueryProvider
{
    private readonly BindingClient2 bindingClient2;
    private readonly CultureOptions cultureOptions;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial GachaLogQuerySTokenProvider(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            return new(false, GachaLogQuery.Invalid(SH.MustSelectUserAndUid));
        }

        if (userAndUid.IsOversea)
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderStokenUnsupported));
        }

        GenAuthKeyData data = GenAuthKeyData.CreateForWebViewGacha(userAndUid.Uid);
        Response<GameAuthKey> authKeyResponse = await bindingClient2.GenerateAuthenticationKeyAsync(userAndUid.User, data).ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(authKeyResponse, messenger, out GameAuthKey? authKey))
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderAuthkeyRequestFailed));
        }

        return new(true, new(ComposeQueryString(data, authKey, cultureOptions.LanguageCode)));
    }

    private static string ComposeQueryString(GenAuthKeyData genAuthKeyData, GameAuthKey gameAuthKey, string lang)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
        collection.Set("lang", lang);
        collection.Set("auth_appid", genAuthKeyData.AuthAppId);
        collection.Set("authkey", gameAuthKey.AuthKey);
        collection.Set("authkey_ver", $"{gameAuthKey.AuthKeyVersion:D}");
        collection.Set("sign_type", $"{gameAuthKey.SignType:D}");

        string? result = collection.ToString();
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }
}
