//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Web.Request.Builder;
using System.Net.Http;
using System.Runtime.ExceptionServices;

namespace kyxsan.Core.IO.Http;

[Service(ServiceLifetime.Transient)]
internal sealed partial class RetryHttpHandler : DelegatingHandler
{
    public static HttpRequestOptionsKey<bool> DisableRetry { get; } = new("DisableRetry");

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Options.TryGetValue(DisableRetry, out bool skipRetry) && skipRetry)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        ExceptionDispatchInfo? dispatch = default;
        int requestCount = 0;
        while (requestCount < 3)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                dispatch = ExceptionDispatchInfo.Capture(ex);
                request.Resurrect();
            }

            requestCount++;
        }

        dispatch?.Throw();
        throw kyxsanException.InvalidOperation("Unexpected request retry state");
    }
}