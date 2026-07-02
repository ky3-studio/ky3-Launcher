//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Web.Endpoint.Launcher;
using System.Net.Http;

namespace Launcher.Core.LifeCycle;

[Service(ServiceLifetime.Transient)]
internal sealed partial class SentryIpAddressEnricher
{
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial SentryIpAddressEnricher(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask ConfigureAsync()
    {
        try
        {
            string ip = await httpClient.GetStringAsync(LauncherEndpointsFactory.Create().IpString()).ConfigureAwait(false);
            SentrySdk.ConfigureScope(static (scope, ip) => { scope.User.IpAddress = ip; }, ip.Trim('"'));
        }
        catch
        {
            // Man, what can I say?
        }
    }
}
