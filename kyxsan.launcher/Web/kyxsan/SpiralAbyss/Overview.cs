//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.kyxsan.SpiralAbyss;

internal sealed class Overview
{
    public int ScheduleId { get; set; }

    public int RecordTotal { get; set; }

    public int SpiralAbyssTotal { get; set; }

    public int SpiralAbyssPassed { get; set; }

    [JsonIgnore]
    public string SpiralAbyssPassedPercent { get => $"{(double)SpiralAbyssPassed / SpiralAbyssTotal:P2}"; }

    public int SpiralAbyssStarTotal { get; set; }

    [JsonIgnore]
    public string SpiralAbyssStarAverage { get => $"{(double)SpiralAbyssStarTotal / SpiralAbyssTotal:F2}"; }

    public int SpiralAbyssFullStar { get; set; }

    [JsonIgnore]
    public string SpiralAbyssFullStarPercent { get => $"{(double)SpiralAbyssFullStar / SpiralAbyssTotal:P2}"; }

    public long SpiralAbyssBattleTotal { get; set; }

    [JsonIgnore]
    public string SpiralAbyssBattleAverage { get => $"{(double)SpiralAbyssBattleTotal / SpiralAbyssTotal:F2}"; }

    public long Timestamp { get; set; }

    [JsonIgnore]
    public string RefreshTime { get => $"{DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).ToLocalTime():MM.dd HH:mm}"; }

    public double TimeTotal { get; set; }

    public double TimeAverage { get; set; }
}