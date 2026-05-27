//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class Transformer
{
    [JsonPropertyName("obtained")]
    [MemberNotNullWhen(true, nameof(RecoveryTime))]
    public bool Obtained { get; set; }

    [JsonPropertyName("recovery_time")]
    public RecoveryTime? RecoveryTime { get; set; }

    public string ObtainedAndReachedFormatted
    {
        get => Obtained ? RecoveryTime.ReachedFormatted : SH.WebDailyNoteTransformerNotObtained;
    }

    public string ObtainedAndTimeFormatted
    {
        get => Obtained ? RecoveryTime.TimeFormatted : SH.WebDailyNoteTransformerNotObtainedDetail;
    }

    [JsonPropertyName("wiki")]
    public Uri Wiki { get; set; } = default!;

    [JsonPropertyName("noticed")]
    public bool Noticed { get; set; }

    [JsonPropertyName("latest_job_id")]
    public string LastJobId { get; set; } = default!;
}
