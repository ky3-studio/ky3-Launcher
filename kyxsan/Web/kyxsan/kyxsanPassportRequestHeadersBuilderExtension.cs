//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Request.Builder.Abstraction;
using System.Net.Http.Headers;

namespace kyxsan.Web.kyxsan;

internal static class kyxsanPassportRequestHeadersBuilderExtension
{
    extension<TBuilder>(TBuilder builder)
        where TBuilder : IHttpHeadersBuilder<HttpRequestHeaders>
    {
        public TBuilder SetAccessToken(string? accessToken)
        {
            builder.Headers.Authorization = string.IsNullOrEmpty(accessToken) ? default : new("Bearer", accessToken);
            return builder;
        }

        public TBuilder SetHomaToken(string? homaToken)
        {
            if (!string.IsNullOrEmpty(homaToken))
            {
                builder.Headers.Add("x-homa-token", homaToken);
            }

            return builder;
        }
    }
}