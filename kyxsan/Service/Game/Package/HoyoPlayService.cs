//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using kyxsan.Service.Game.Scheme;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using kyxsan.Web.Response;

namespace kyxsan.Service.Game.Package;

[Service(ServiceLifetime.Singleton, typeof(IHoyoPlayService))]
internal sealed partial class HoyoPlayService : IHoyoPlayService
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial HoyoPlayService(IServiceProvider serviceProvider);

    public ValueTask<ValueResult<bool, GameBranchesWrapper>> TryGetBranchesAsync(LaunchScheme scheme)
    {
        return TryGetAsync(scheme, static (client, scheme) => client.GetBranchesAsync(scheme));
    }

    public ValueTask<ValueResult<bool, GameChannelSDKsWrapper>> TryGetChannelSDKsAsync(LaunchScheme scheme)
    {
        return TryGetAsync(scheme, static (client, scheme) => client.GetChannelSDKAsync(scheme));
    }

    public ValueTask<ValueResult<bool, DeprecatedFileConfigurationsWrapper>> TryGetDeprecatedFileConfigurationsAsync(LaunchScheme scheme)
    {
        return TryGetAsync(scheme, static (client, scheme) => client.GetDeprecatedFileConfigurationsAsync(scheme));
    }

    private async ValueTask<ValueResult<bool, T>> TryGetAsync<T>(LaunchScheme scheme, [RequireStaticDelegate] Func<HoyoPlayClient, LaunchScheme, ValueTask<Response<T>>> asyncMethod)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();

            Response<T> response = await asyncMethod(hoyoPlayClient, scheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, serviceProvider, out T? data))
            {
                return new(false, default!);
            }

            return new(true, data);
        }
    }
}