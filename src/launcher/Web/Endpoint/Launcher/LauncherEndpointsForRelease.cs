//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

namespace Launcher.Web.Endpoint.Launcher;

[Service(ServiceLifetime.Singleton, typeof(ILauncherEndpoints), Key = LauncherEndpointsKind.Release)]
internal sealed class LauncherEndpointsForRelease : ILauncherEndpoints
{
    string IHomaRootAccess.Root { get => Service.RemoteConfig.BackendApiRoutes.ApiBase; }

    string IInfrastructureRootAccess.Root { get => Service.RemoteConfig.BackendApiRoutes.ApiBase; }

    string IInfrastructureRawRootAccess.RawRoot { get => Service.RemoteConfig.BackendApiRoutes.ApiBase; }
}