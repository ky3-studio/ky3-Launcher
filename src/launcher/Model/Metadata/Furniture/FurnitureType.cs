//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Primitive;

namespace kyxsan.Model.Metadata.Furniture;

internal sealed class FurnitureType
{
    public required FurnitureTypeId Id { get; init; }

    public required uint Category { get; init; }

    public required string Name { get; init; }

    public required string Name2 { get; init; }

    public required string TabIcon { get; init; }

    public required FurnitureDeployType SceneType { get; init; }

    public required bool BagPageOnly { get; init; }

    public required bool IsShowInBag { get; init; }

    public required uint Sort { get; init; }
}