//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Request.Builder;

namespace kyxsan.Web.Hoyolab.DataSigning;

internal static class DataSignHttpRequestMessageBuilderExtension
{
    extension(HttpRequestMessageBuilder builder)
    {
        public async ValueTask SignDataAsync(DataSignAlgorithmVersion version, SaltType saltType, bool includeChars)
        {
            DataSignOptions options = await DataSignOptions.CreateFromHttpRequestMessageBuilderAsync(builder, saltType, includeChars, version).ConfigureAwait(false);
            builder.SetHeader("DS", DataSignAlgorithm.GetDataSign(options));
        }
    }
}