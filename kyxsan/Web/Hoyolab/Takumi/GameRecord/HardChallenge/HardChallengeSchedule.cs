//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeSchedule
{
    [JsonPropertyName("schedule_id")]
    public required HardChallengeScheduleId ScheduleId { get; init; }

    [JsonPropertyName("start_time")]
    public required long StartTime { get; init; }

    [JsonPropertyName("end_time")]
    public required long EndTime { get; init; }

    [JsonPropertyName("start_date_time")]
    public required DateTime StartDateTime { get; init; }

    [JsonPropertyName("end_date_time")]
    public required DateTime EndDateTime { get; init; }

    [JsonPropertyName("is_valid")]
    public required bool IsValid { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}