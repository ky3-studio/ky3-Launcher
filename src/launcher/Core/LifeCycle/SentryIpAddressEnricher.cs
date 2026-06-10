//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Endpoint.kyxsan;
using System.Net.Http;

namespace kyxsan.Core.LifeCycle;

[Service(ServiceLifetime.Transient)]
internal sealed partial class SentryIpAddressEnricher
{
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial SentryIpAddressEnricher(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask ConfigureAsync()
    {
        try
        {
            string ip = await httpClient.GetStringAsync(kyxsanEndpointsFactory.Create().IpString()).ConfigureAwait(false);
            SentrySdk.ConfigureScope(static (scope, ip) => { scope.User.IpAddress = ip; }, ip.Trim('"'));
        }
        catch
        {
            // Man, what can I say?
        }
    }
}