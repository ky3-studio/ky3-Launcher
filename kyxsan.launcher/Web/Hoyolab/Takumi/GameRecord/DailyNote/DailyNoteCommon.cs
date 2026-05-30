//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal abstract class DailyNoteCommon
{
    [JsonPropertyName("current_expedition_num")]
    public int CurrentExpeditionNum { get; init; }

    [JsonPropertyName("current_home_coin")]
    public int CurrentHomeCoin { get; init; }

    [JsonPropertyName("current_resin")]
    public int CurrentResin { get; init; }

    [JsonPropertyName("expeditions")]
    public List<Expedition> Expeditions { get; init; } = default!;

    [JsonPropertyName("finished_task_num")]
    public int FinishedTaskNum { get; init; }

    [JsonPropertyName("is_extra_task_reward_received")]
    public bool IsExtraTaskRewardReceived { get; init; }

    [JsonPropertyName("max_expedition_num")]
    public int MaxExpeditionNum { get; init; }

    [JsonPropertyName("max_home_coin")]
    public int MaxHomeCoin { get; init; }

    [JsonPropertyName("max_resin")]
    public int MaxResin { get; init; }

    [JsonPropertyName("resin_recovery_time")]
    public int ResinRecoveryTime { get; init; }

    [JsonPropertyName("total_task_num")]
    public int TotalTaskNum { get; init; }
}