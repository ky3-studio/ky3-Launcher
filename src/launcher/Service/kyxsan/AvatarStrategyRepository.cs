//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Model.Primitive;
using kyxsan.Service.Abstraction;

namespace kyxsan.Service.kyxsan;

[Service(ServiceLifetime.Singleton, typeof(IAvatarStrategyRepository))]
internal sealed partial class AvatarStrategyRepository : IAvatarStrategyRepository
{
    [GeneratedConstructor]
    public partial AvatarStrategyRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public AvatarStrategy? GetStrategyByAvatarId(AvatarId avatarId)
    {
        return this.SingleOrDefault(s => s.AvatarId == (uint)avatarId);
    }

    public void AddStrategy(AvatarStrategy strategy)
    {
        this.Add(strategy);
    }

    public void RemoveStrategy(AvatarStrategy strategy)
    {
        this.Delete(strategy);
    }
}