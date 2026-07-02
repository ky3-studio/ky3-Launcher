//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Model.Primitive;
using System.Collections.Immutable;

namespace Launcher.Model.Metadata;

internal sealed class Combine
{
    public required CombineId Id { get; init; }

    public required uint Type { get; init; }

    public required uint SubType { get; init; }

    public required RecipeType RecipeType { get; init; }

    public required uint Cost { get; init; }

    public required IdCount Result { get; init; }

    public required ImmutableArray<IdCount> Materials { get; init; }

    public string EffectDescription { get; init; } = default!;
}
