//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class ArchonQuestProgress
{
    [JsonPropertyName("list")]
    public List<ArchonQuest> List { get; set; } = default!;

    [JsonPropertyName("is_open_archon_quest")]
    public bool IsOpenArchonQuest { get; set; }

    [JsonPropertyName("is_finish_all_mainline")]
    public bool IsFinishAllMainline { get; set; }

    [JsonPropertyName("is_finish_all_interchapter")]
    public bool IsFinishAllInterchapter { get; set; }

    [JsonPropertyName("wiki_url")]
    public string WikiUrl { get; set; } = default!;
}