//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Model.Entity.Primitive;
using kyxsan.Service.Game.Account;
using kyxsan.Service.Game.Configuration;
using kyxsan.Service.Game.PathAbstraction;
using kyxsan.UI.Xaml.Data;

namespace kyxsan.Service.Game;

[Service(ServiceLifetime.Singleton, typeof(IGameService))]
internal sealed partial class GameService : IGameService
{
    private readonly IGameInRegistryAccountService gameInRegistryAccountService;
    private readonly IGameChannelOptionsService gameChannelOptionsService;
    private readonly IGamePathService gamePathService;

    [GeneratedConstructor]
    public partial GameService(IServiceProvider serviceProvider);

    public ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync()
    {
        return gameInRegistryAccountService.GetGameAccountCollectionAsync();
    }

    public ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        return gamePathService.SilentLocateGamePathAsync();
    }

    public ChannelOptions GetChannelOptions()
    {
        return gameChannelOptionsService.GetChannelOptions();
    }

    public ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        return gameInRegistryAccountService.DetectCurrentGameAccountAsync(scheme, providerNameCallback);
    }

    public GameAccount? DetectCurrentGameAccount(SchemeType scheme)
    {
        return gameInRegistryAccountService.DetectCurrentGameAccount(scheme);
    }

    public ValueTask ModifyGameAccountAsync(GameAccount gameAccount, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        return gameInRegistryAccountService.ModifyGameAccountAsync(gameAccount, providerNameCallback);
    }

    public ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        return gameInRegistryAccountService.RemoveGameAccountAsync(gameAccount);
    }
}