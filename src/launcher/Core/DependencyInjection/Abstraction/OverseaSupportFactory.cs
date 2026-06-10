//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.DependencyInjection.Abstraction;

internal abstract partial class OverseaSupportFactory<TClient, TClientCN, TClientOS> : IOverseaSupportFactory<TClient>
    where TClient : notnull
    where TClientCN : TClient
    where TClientOS : TClient
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial OverseaSupportFactory(IServiceProvider serviceProvider);

    public TClient Create(bool isOversea)
    {
        return isOversea
            ? serviceProvider.GetRequiredService<TClientOS>()
            : serviceProvider.GetRequiredService<TClientCN>();
    }
}