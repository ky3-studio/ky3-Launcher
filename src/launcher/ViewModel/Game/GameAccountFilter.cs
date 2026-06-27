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

namespace Launcher.ViewModel.Game;

internal sealed class GameAccountFilter
{
    private readonly SchemeType? type;

    private GameAccountFilter(SchemeType? type)
    {
        this.type = type;
    }

    public static Predicate<GameAccount> Create(SchemeType? type)
    {
        return new GameAccountFilter(type).Filter;
    }

    public bool Filter(GameAccount? item)
    {
        if (type is null)
        {
            return true;
        }

        return item is not null && item.Type == type;
    }
}