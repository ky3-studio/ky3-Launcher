//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace kyxsan.Web.Request.Builder;

internal static class HttpContentDeserializerExtension
{
    extension(IHttpContentDeserializer deserializer)
    {
        public async ValueTask<T?> DeserializeAsync<T>(HttpContent? httpContent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deserializer);
            return (T?)await deserializer.DeserializeAsync(httpContent, typeof(T), cancellationToken).ConfigureAwait(false);
        }
    }
}