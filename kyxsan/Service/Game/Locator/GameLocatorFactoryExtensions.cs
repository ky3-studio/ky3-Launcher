//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Service.Game.Locator;

internal static class GameLocatorFactoryExtensions
{
    extension(IGameLocatorFactory factory)
    {
        public ValueTask<ValueResult<bool, string>> LocateSingleAsync(GameLocationSourceKind source)
        {
            return factory.Create(source).LocateSingleGamePathAsync();
        }

        public ValueTask<ImmutableArray<string>> LocateMultipleAsync(GameLocationSourceKind source)
        {
            IGameLocator locator = factory.Create(source);
            if (locator is IGameLocator2 locator2)
            {
                return locator2.LocateMultipleGamePathAsync();
            }

            return ValueTask.FromResult(ImmutableArray<string>.Empty);
        }
    }
}