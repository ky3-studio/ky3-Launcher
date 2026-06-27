//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Model.Entity;
using Launcher.Service.Game.Scheme;

namespace Launcher.Service.Game;

internal static class GameServiceExtension
{
    extension(IGameService gameService)
    {
        public GameAccount? DetectCurrentGameAccountNoThrow(LaunchScheme scheme)
        {
            try
            {
                return gameService.DetectCurrentGameAccount(scheme.SchemeType);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return default;
            }
        }

        public ValueTask<GameAccount?> DetectGameAccountAsync(LaunchScheme scheme, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
        {
            return gameService.DetectGameAccountAsync(scheme.SchemeType, providerNameCallback);
        }
    }
}