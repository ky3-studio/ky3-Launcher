//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Model.Entity;
using Launcher.Service.Abstraction;

namespace Launcher.Service.Game.Account;

[Service(ServiceLifetime.Singleton, typeof(IGameAccountRepository))]
internal sealed partial class GameAccountRepository : IGameAccountRepository
{
    [GeneratedConstructor]
    public partial GameAccountRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public ObservableReorderableDbCollection<GameAccount> GetGameAccountCollection()
    {
        return this.Query(query => query.ToObservableReorderableDbCollection(ServiceProvider));
    }

    public void AddGameAccount(GameAccount gameAccount)
    {
        this.Add(gameAccount);
    }

    public void UpdateGameAccount(GameAccount gameAccount)
    {
        this.Update(gameAccount);
    }

    public void RemoveGameAccountById(Guid id)
    {
        this.DeleteByInnerId(id);
    }
}
