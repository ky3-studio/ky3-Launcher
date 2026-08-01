//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Entity;
using Launcher.Model.Primitive;

namespace Launcher.Service.Launcher;

[Service(ServiceLifetime.Singleton, typeof(IAvatarStrategyService))]
internal sealed partial class AvatarStrategyService : IAvatarStrategyService
{
    private readonly IAvatarStrategyRepository repository;

    [GeneratedConstructor]
    public partial AvatarStrategyService(IServiceProvider serviceProvider);

    public ValueTask<AvatarStrategy?> GetStrategyByAvatarId(AvatarId avatarId)
    {
        AvatarStrategy? strategy = repository.GetStrategyByAvatarId(avatarId);
        if (strategy is { ChineseStrategyId: 0 } or { OverseaStrategyId: 0 })
        {
            repository.RemoveStrategy(strategy);
            strategy = default;
        }

        return ValueTask.FromResult(strategy);
    }
}
