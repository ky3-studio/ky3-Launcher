//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Abstraction;
using Launcher.Web.Request.Builder.Abstraction;
using System.Diagnostics;

namespace Launcher.Web.Request.Builder;

internal static class RequestUriBuilderExtension
{
    extension<T>(T builder)
        where T : class, IRequestUriBuilder
    {
        [DebuggerStepThrough]
        public T SetRequestUri(string? requestUri, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            return builder.SetRequestUri(string.IsNullOrEmpty(requestUri) ? null : new Uri(requestUri, uriKind));
        }

        [DebuggerStepThrough]
        public T SetRequestUri(Uri? requestUri)
        {
            return builder.Configure(builder => builder.RequestUri = requestUri);
        }
    }
}