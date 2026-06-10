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
using kyxsan.UI.Xaml.Data;

namespace kyxsan.Service.Game.Account;

internal interface IGameInRegistryAccountService
{
    GameAccount? DetectCurrentGameAccount(SchemeType schemeType);

    ValueTask<GameAccount?> DetectCurrentGameAccountAsync(SchemeType schemeType, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback);

    ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync();

    ValueTask ModifyGameAccountAsync(GameAccount gameAccount, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback);

    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    bool SetCurrentGameAccount(GameAccount account);
}