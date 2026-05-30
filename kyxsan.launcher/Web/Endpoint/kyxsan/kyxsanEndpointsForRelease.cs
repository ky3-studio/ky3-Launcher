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
    private const string ApiRoot = "https://8.134.75.17:9000/api";

    string IHomaRootAccess.Root { get => ApiRoot; }

    string IInfrastructureRootAccess.Root { get => ApiRoot; }

    string IInfrastructureRawRootAccess.RawRoot { get => ApiRoot; }
}