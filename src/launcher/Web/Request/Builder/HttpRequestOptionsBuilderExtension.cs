//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
using kyxsan.Core.Abstraction;
using kyxsan.Web.Request.Builder.Abstraction;
using System.Diagnostics;
using System.Net.Http;

namespace kyxsan.Web.Request.Builder;

internal static class HttpRequestOptionsBuilderExtension
{
    extension<TBuilder>(TBuilder builder)
        where TBuilder : class, IHttpRequestOptionsBuilder
    {
        [DebuggerStepThrough]
        public TBuilder SetOptions<TValue>(HttpRequestOptionsKey<TValue> key, TValue value)
        {
            return builder.Configure(builder => builder.Options.Set(key, value));
        }
    }
}