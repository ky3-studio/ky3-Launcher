//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.ViewModel.User;
using kyxsan.Web.Request.Builder;
using System.Text;

namespace kyxsan.Web.Hoyolab;

internal static class HoyolabHttpRequestMessageBuilderExtension
{
    extension(HttpRequestMessageBuilder builder)
    {
        internal HttpRequestMessageBuilder SetUserCookieAndFpHeader(UserAndUid userAndUid, CookieType cookie)
        {
            return builder.SetUserCookieAndFpHeader(userAndUid.User, cookie);
        }

        internal HttpRequestMessageBuilder SetUserCookieAndFpHeader(Model.Entity.User user, CookieType cookie)
        {
            builder.RemoveHeader("Cookie");
            StringBuilder stringBuilder = new();

            if (cookie.HasFlag(CookieType.CookieToken))
            {
                stringBuilder.Append(user.CookieToken).AppendIf(user.CookieToken is not null, ';');
            }

            if (cookie.HasFlag(CookieType.LToken))
            {
                stringBuilder.Append(user.LToken).AppendIf(user.LToken is not null, ';');
            }

            if (cookie.HasFlag(CookieType.SToken))
            {
                stringBuilder.Append(user.SToken).AppendIf(user.SToken is not null, ';');
            }

            string result = stringBuilder.ToString();
            if (!string.IsNullOrWhiteSpace(result))
            {
                builder.AddHeader("Cookie", result);
            }

            if (!string.IsNullOrEmpty(user.Fingerprint))
            {
                builder.SetHeader("x-rpc-device_fp", user.Fingerprint);
            }

            return builder;
        }

        internal HttpRequestMessageBuilder SetXrpcAigis(string aigis)
        {
            return builder.SetHeader("x-rpc-aigis", aigis);
        }

        internal HttpRequestMessageBuilder SetXrpcChallenge(string challenge)
        {
            return builder.SetHeader("x-rpc-challenge", challenge);
        }

        internal HttpRequestMessageBuilder SetXrpcChallenge(string challenge, string validate)
        {
            return builder
                .SetHeader("x-rpc-challenge", challenge)
                .SetHeader("x-rpc-validate", validate)
                .SetHeader("x-rpc-seccode", $"{validate}|jordan");
        }

        internal HttpRequestMessageBuilder SetXrpcVerify(string verify)
        {
            return builder.SetHeader("x-rpc-verify", verify);
        }
    }
}