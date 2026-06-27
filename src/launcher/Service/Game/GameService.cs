//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Entity;
using Launcher.Model.Entity.Primitive;
using Launcher.Service.Game.Account;
using Launcher.Service.Game.Configuration;
using Launcher.Service.Game.PathAbstraction;
using Launcher.UI.Xaml.Data;

namespace Launcher.Service.Game;

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