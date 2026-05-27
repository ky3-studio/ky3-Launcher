//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.kyxsan.SpiralAbyss.Post;

internal sealed class SimpleSpiralAbyss
{
    public SimpleSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        ScheduleId = spiralAbyss.ScheduleId;
        TotalBattleTimes = spiralAbyss.TotalBattleTimes;
        TotalWinTimes = spiralAbyss.TotalWinTimes;
        Damage = SimpleRank.FromRank(spiralAbyss.DamageRank.SingleOrDefault());
        TakeDamage = SimpleRank.FromRank(spiralAbyss.TakeDamageRank.SingleOrDefault());
        Floors = spiralAbyss.Floors.Select(f => new SimpleFloor(f));
    }

    public uint ScheduleId { get; set; }

    public int TotalBattleTimes { get; set; }

    public int TotalWinTimes { get; set; }

    public SimpleRank? Damage { get; set; }

    public SimpleRank? TakeDamage { get; set; }

    public IEnumerable<SimpleFloor> Floors { get; set; }
}