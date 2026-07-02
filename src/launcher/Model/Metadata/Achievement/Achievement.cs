//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Primitive;

namespace Launcher.Model.Metadata.Achievement;

internal sealed class Achievement : IDefaultIdentity<AchievementId>
{
    public required AchievementId Id { get; init; }

    public required AchievementGoalId Goal { get; init; }

    public required uint Order { get; init; }

    public AchievementId PreviousId { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public required Reward FinishReward { get; init; }

    public bool IsDeleteWatcherAfterFinish { get; init; }

    public required uint Progress { get; init; }

    public string? Icon { get; init; }

    public required string Version { get; init; }

    public bool IsDailyQuest { get; init; }
}
