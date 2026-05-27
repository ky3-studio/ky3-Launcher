//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Service.Yae.Achievement;

internal sealed class TargetNativeConfiguration
{
    public required uint StoreCmdId { get; init; }

    public required uint AchievementCmdId { get; init; }

    public required uint DoCmd { get; init; }

    public required uint UpdateNormalProperty { get; init; }

    public required uint NewString { get; init; }

    public required uint FindGameObject { get; init; }

    public required uint EventSystemUpdate { get; init; }

    public required uint SimulatePointerClick { get; init; }

    public required uint ToInt32 { get; init; }

    public required uint TcpStatePtr { get; init; }

    public required uint SharedInfoPtr { get; init; }

    public required uint Decompress { get; init; }
}