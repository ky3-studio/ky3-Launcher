//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Primitive;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Launcher.Model.Metadata.Item;

internal static class MaterialIds
{
    public const uint Mora = 202U;                       // 摩拉
    public const uint WanderersAdvice = 104001U;         // 流浪者的经验
    public const uint AdventurersExperience = 104002U;   // 冒险家的经验
    public const uint HeroesWit = 104003U;               // 大英雄的经验
    public const uint MysticEnhancementOre = 104013U;    // 精锻用魔矿
    public const uint AgnidusAgateSliver = 104111U;      // 燃愿玛瑙碎屑
    public const uint VarunadaLazuriteSliver = 104121U;  // 涤净青金碎屑
    public const uint NagadusEmeraldSliver = 104131U;    // 生长碧翡碎屑
    public const uint VajradaAmethystSliver = 104141U;   // 最胜紫晶碎屑
    public const uint VayudaTurquoiseSliver = 104151U;   // 自在松石碎屑
    public const uint ShivadaJadeSliver = 104161U;       // 哀叙冰玉碎屑
    public const uint PrithivaTopazSliver = 104171U;     // 坚牢黄玉碎屑
    public const uint MasterlessStellaFortuna = 104300U; // 无主的命星
    public const uint CrownOfInsight = 104319U;          // 智识之冕

    private static readonly ImmutableArray<RotationalMaterialIdEntry> Entries =
    [
        new(DaysOfWeek.MondayAndThursday, 104301U, 104302U, 104303U), // 「自由」
        new(DaysOfWeek.MondayAndThursday, 104310U, 104311U, 104312U), // 「繁荣」
        new(DaysOfWeek.MondayAndThursday, 104320U, 104321U, 104322U), // 「浮世」
        new(DaysOfWeek.MondayAndThursday, 104329U, 104330U, 104331U), // 「诤言」
        new(DaysOfWeek.MondayAndThursday, 104338U, 104339U, 104340U), // 「公平」
        new(DaysOfWeek.MondayAndThursday, 104347U, 104348U, 104349U), // 「角逐」
        new(DaysOfWeek.MondayAndThursday, 104356U, 104357U, 104358U), // 「月光」
        new(DaysOfWeek.TuesdayAndFriday, 104304U, 104305U, 104306U), // 「抗争」
        new(DaysOfWeek.TuesdayAndFriday, 104313U, 104314U, 104315U), // 「勤劳」
        new(DaysOfWeek.TuesdayAndFriday, 104323U, 104324U, 104325U), // 「风雅」
        new(DaysOfWeek.TuesdayAndFriday, 104332U, 104333U, 104334U), // 「巧思」
        new(DaysOfWeek.TuesdayAndFriday, 104341U, 104342U, 104343U), // 「正义」
        new(DaysOfWeek.TuesdayAndFriday, 104350U, 104351U, 104352U), // 「焚燔」
        new(DaysOfWeek.TuesdayAndFriday, 104359U, 104360U, 104361U), // 「乐园」
        new(DaysOfWeek.WednesdayAndSaturday, 104307U, 104308U, 104309U), // 「诗文」
        new(DaysOfWeek.WednesdayAndSaturday, 104316U, 104317U, 104318U), // 「黄金」
        new(DaysOfWeek.WednesdayAndSaturday, 104326U, 104327U, 104328U), // 「天光」
        new(DaysOfWeek.WednesdayAndSaturday, 104335U, 104336U, 104337U), // 「笃行」
        new(DaysOfWeek.WednesdayAndSaturday, 104344U, 104345U, 104346U), // 「秩序」
        new(DaysOfWeek.WednesdayAndSaturday, 104353U, 104354U, 104355U), // 「纷争」
        new(DaysOfWeek.WednesdayAndSaturday, 104362U, 104363U, 104364U), // 「浪迹」
        new(DaysOfWeek.MondayAndThursday, 114001U, 114002U, 114003U, 114004U), // 高塔孤王
        new(DaysOfWeek.MondayAndThursday, 114013U, 114014U, 114015U, 114016U), // 孤云寒林
        new(DaysOfWeek.MondayAndThursday, 114025U, 114026U, 114027U, 114028U), // 远海夷地
        new(DaysOfWeek.MondayAndThursday, 114037U, 114038U, 114039U, 114040U), // 谧林涓露
        new(DaysOfWeek.MondayAndThursday, 114049U, 114050U, 114051U, 114052U), // 悠古弦音
        new(DaysOfWeek.MondayAndThursday, 114061U, 114062U, 114063U, 114064U), // 贡祭炽心
        new(DaysOfWeek.MondayAndThursday, 114073U, 114074U, 114075U, 114076U), // 奇巧秘器
        new(DaysOfWeek.TuesdayAndFriday, 114005U, 114006U, 114007U, 114008U), // 凛风奔狼
        new(DaysOfWeek.TuesdayAndFriday, 114017U, 114018U, 114019U, 114020U), // 雾海云间
        new(DaysOfWeek.TuesdayAndFriday, 114029U, 114030U, 114031U, 114032U), // 鸣神御灵
        new(DaysOfWeek.TuesdayAndFriday, 114041U, 114042U, 114043U, 114044U), // 绿洲花园
        new(DaysOfWeek.TuesdayAndFriday, 114053U, 114054U, 114055U, 114056U), // 纯圣露滴
        new(DaysOfWeek.TuesdayAndFriday, 114065U, 114066U, 114067U, 114068U), // 谵妄圣主
        new(DaysOfWeek.TuesdayAndFriday, 114077U, 114078U, 114079U, 114080U), // 长夜燧火
        new(DaysOfWeek.WednesdayAndSaturday, 114009U, 114010U, 114011U, 114012U), // 狮牙斗士
        new(DaysOfWeek.WednesdayAndSaturday, 114021U, 114022U, 114023U, 114024U), // 漆黑陨铁
        new(DaysOfWeek.WednesdayAndSaturday, 114033U, 114034U, 114035U, 114036U), // 今昔剧画
        new(DaysOfWeek.WednesdayAndSaturday, 114045U, 114046U, 114047U, 114048U), // 谧林涓露
        new(DaysOfWeek.WednesdayAndSaturday, 114057U, 114058U, 114059U, 114060U), // 无垢之海
        new(DaysOfWeek.WednesdayAndSaturday, 114069U, 114070U, 114071U, 114072U), // 神合秘烟
        new(DaysOfWeek.WednesdayAndSaturday, 114081U, 114082U, 114083U, 114084U), // 终北遗嗣
    ];

    public static FrozenSet<MaterialId> MondayThursdayItems { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.MondayAndThursday).SelectMany(entry => entry.Enumerate())];

    public static FrozenSet<RotationalMaterialIdEntry> MondayThursdayEntries { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.MondayAndThursday)];

    public static FrozenSet<MaterialId> TuesdayFridayItems { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.TuesdayAndFriday).SelectMany(entry => entry.Enumerate())];

    public static FrozenSet<RotationalMaterialIdEntry> TuesdayFridayEntries { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.TuesdayAndFriday)];

    public static FrozenSet<MaterialId> WednesdaySaturdayItems { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.WednesdayAndSaturday).SelectMany(entry => entry.Enumerate())];

    public static FrozenSet<RotationalMaterialIdEntry> WednesdaySaturdayEntries { get; } = [.. Entries.Where(entry => entry.DaysOfWeek is DaysOfWeek.WednesdayAndSaturday)];

    public static DaysOfWeek GetDaysOfWeek(MaterialId id)
    {
        if (MondayThursdayItems.Contains(id))
        {
            return DaysOfWeek.MondayAndThursday;
        }

        if (TuesdayFridayItems.Contains(id))
        {
            return DaysOfWeek.TuesdayAndFriday;
        }

        if (WednesdaySaturdayItems.Contains(id))
        {
            return DaysOfWeek.WednesdayAndSaturday;
        }

        return DaysOfWeek.Any;
    }

    public static DaysOfWeek GetDaysOfWeek(ReadOnlySpan<MaterialId> ids)
    {
        if (ids.IsEmpty)
        {
            return DaysOfWeek.Any;
        }

        DaysOfWeek first = GetDaysOfWeek(ids[0]);
        foreach (ref readonly MaterialId id in ids[1..])
        {
            if (GetDaysOfWeek(id) != first)
            {
                return DaysOfWeek.Any;
            }
        }

        return first;
    }
}