//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Endpoint.kyxsan;

[Service(ServiceLifetime.Singleton, typeof(IkyxsanEndpoints), Key = kyxsanEndpointsKind.Release)]
internal sealed class kyxsanEndpointsForRelease : IkyxsanEndpoints
{
    string IHomaRootAccess.Root { get => Service.RemoteConfig.BackendApiRoutes.ApiBase; }

    string IInfrastructureRootAccess.Root { get => Service.RemoteConfig.BackendApiRoutes.ApiBase; }

    string IInfrastructureRawRootAccess.RawRoot { get => Service.RemoteConfig.BackendApiRoutes.ApiBase; }
}