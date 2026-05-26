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
using kyxsan.Web.kyxsan.Strategy;
using kyxsan.Web.Response;
using System.Collections.Immutable;

namespace kyxsan.Service.kyxsan;

[Service(ServiceLifetime.Singleton, typeof(IAvatarStrategyService))]
internal sealed partial class AvatarStrategyService : IAvatarStrategyService
{
    private readonly IAvatarStrategyRepository repository;
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial AvatarStrategyService(IServiceProvider serviceProvider);

    public async ValueTask<AvatarStrategy?> GetStrategyByAvatarId(AvatarId avatarId)
    {
        AvatarStrategy? strategy = repository.GetStrategyByAvatarId(avatarId);
        if (strategy is { ChineseStrategyId: 0 } or { OverseaStrategyId: 0 })
        {
            repository.RemoveStrategy(strategy);
            strategy = default;
        }

        if (strategy is null)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                kyxsanStrategyClient strategyClient = scope.ServiceProvider.GetRequiredService<kyxsanStrategyClient>();
                Response<ImmutableDictionary<AvatarId, Strategy>> response = await strategyClient.GetStrategyItemAsync(avatarId).ConfigureAwait(false);

                if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out ImmutableDictionary<AvatarId, Strategy>? dictionary))
                {
                    if (!dictionary.TryGetValue(avatarId, out Strategy? data))
                    {
                        return default;
                    }

                    if (data.HoyolabStrategyId is null && data.MysStrategyId is null)
                    {
                        return default;
                    }

                    strategy = new()
                    {
                        AvatarId = avatarId,
                        ChineseStrategyId = data.MysStrategyId ?? 0,
                        OverseaStrategyId = data.HoyolabStrategyId ?? 0,
                    };

                    repository.AddStrategy(strategy);
                }
            }
        }

        return strategy;
    }
}