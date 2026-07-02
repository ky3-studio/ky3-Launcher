//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Property;
using System.Collections.Immutable;

namespace Launcher.Service.Game;

internal static class AspectRatioExtension
{
    extension(IProperty<ImmutableArray<AspectRatio>> aspectRatios)
    {
        public ImmutableArray<AspectRatio> Add(AspectRatio aspectRatio)
        {
            if (!aspectRatios.Value.Contains(aspectRatio))
            {
                aspectRatios.Value = aspectRatios.Value.Add(aspectRatio);
            }

            return aspectRatios.Value;
        }

        public ImmutableArray<AspectRatio> Remove(AspectRatio aspectRatio)
        {
            return aspectRatios.Value = aspectRatios.Value.Remove(aspectRatio);
        }
    }
}
