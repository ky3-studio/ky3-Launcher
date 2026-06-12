// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Item;
using kyxsan.ViewModel.Cultivation;

namespace kyxsan.Service.Cultivation;

internal static class ResinEstimator
{
    private const int DailyResin = 180;
    private const int LeyLineCost = 20;
    private const int NormalBossCost = 40;
    private const int WeeklyBossCost = 30;
    private const int TalentDomainCost = 20;

    public static List<ResinEstimationItem> Estimate(IReadOnlyList<StatisticsCultivateItem> statistics, int worldLevel)
    {
        MaterialDropDistribution dist = GetDistribution(worldLevel);
        List<ResinEstimationItem> result = [];

        double remainingMora = 0;
        double remainingExp = 0;
        double talentGreenEquiv = 0;
        double normalBossRemaining = 0;
        double weeklyBossRemaining = 0;

        foreach (StatisticsCultivateItem item in statistics)
        {
            if (item.IsFinished)
            {
                continue;
            }

            double remaining = Math.Max(0, (double)item.Count - item.Current);
            if (remaining <= 0)
            {
                continue;
            }

            uint id = (uint)item.Material.Id;

            if (id == MaterialIds.Mora)
            {
                remainingMora += remaining;
            }
            else if (id is MaterialIds.HeroesWit or MaterialIds.AdventurersExperience or MaterialIds.WanderersAdvice)
            {
                int expPerUnit = id switch
                {
                    MaterialIds.HeroesWit => 20000,
                    MaterialIds.AdventurersExperience => 5000,
                    _ => 1000,
                };
                remainingExp += remaining * expPerUnit;
            }
            else if (item.Material.TypeDescription == SH.ModelMetadataMaterialCharacterTalentMaterial)
            {
                talentGreenEquiv += remaining * QualityMultiplier(item.Material.RankLevel);
            }
            else if (item.Material.TypeDescription == SH.ModelMetadataMaterialCharacterLevelUpMaterial)
            {
                // RankLevel 5 = Weekly Boss, RankLevel 4 = Normal Boss
                if (item.Material.RankLevel is QualityType.QUALITY_ORANGE or QualityType.QUALITY_ORANGE_SP)
                {
                    weeklyBossRemaining += remaining;
                }
                else
                {
                    normalBossRemaining += remaining;
                }
            }
            else
            {
                // 其余材料不计入树脂预估（如地方特产、普通掉落等）
            }
        }

        // Talent domain calculation
        uint talentRuns = 0;
        uint talentResin = 0;
        if (talentGreenEquiv > 0 && dist.TalentBooks > 0)
        {
            talentRuns = (uint)Math.Ceiling(talentGreenEquiv / dist.TalentBooks);
            talentResin = talentRuns * TalentDomainCost;
            remainingMora -= talentRuns * dist.TalentBooksMora;
        }

        // Normal boss calculation
        uint normalBossRuns = 0;
        uint normalBossResin = 0;
        if (normalBossRemaining > 0 && dist.NormalBoss > 0)
        {
            normalBossRuns = (uint)Math.Ceiling(normalBossRemaining / dist.NormalBoss);
            normalBossResin = normalBossRuns * NormalBossCost;
            remainingMora -= normalBossRuns * dist.NormalBossMora;
        }

        // Weekly boss calculation
        uint weeklyBossRuns = 0;
        uint weeklyBossResin = 0;
        if (weeklyBossRemaining > 0 && dist.WeeklyBoss > 0)
        {
            weeklyBossRuns = (uint)Math.Ceiling(weeklyBossRemaining / dist.WeeklyBoss);
            weeklyBossResin = weeklyBossRuns * WeeklyBossCost;
            remainingMora -= weeklyBossRuns * dist.WeeklyBossMora;
        }

        // EXP ley line runs (Blossom of Revelation)
        uint expRuns = 0;
        uint expResin = 0;
        if (remainingExp > 0 && dist.BlossomOfRevelation > 0)
        {
            expRuns = (uint)Math.Ceiling(remainingExp / dist.BlossomOfRevelation);
            expResin = expRuns * LeyLineCost;
        }

        // Mora ley line runs (deduct mora earned from other domains)
        remainingMora = Math.Max(0, remainingMora);
        uint moraRuns = 0;
        uint moraResin = 0;
        if (remainingMora > 0 && dist.BlossomOfWealth > 0)
        {
            moraRuns = (uint)Math.Ceiling(remainingMora / dist.BlossomOfWealth);
            moraResin = moraRuns * LeyLineCost;
        }

        // 藏金之花 = Mora ley line + EXP ley line combined
        uint leyLineTotalResin = moraResin + expResin;
        uint leyLineTotalRuns = moraRuns + expRuns;
        if (leyLineTotalResin > 0)
        {
            result.Add(new()
            {
                Category = SH.ViewPageCultivationResinBlossomOfWealth,
                TotalResin = leyLineTotalResin,
                Runs = leyLineTotalRuns,
                Days = (uint)Math.Ceiling((double)leyLineTotalResin / DailyResin),
            });
        }

        if (talentResin > 0)
        {
            result.Add(new()
            {
                Category = SH.ViewPageCultivationResinTalentDomain,
                TotalResin = talentResin,
                Runs = talentRuns,
                Days = (uint)Math.Ceiling((double)talentResin / DailyResin),
            });
        }

        if (normalBossResin > 0)
        {
            result.Add(new()
            {
                Category = SH.ViewPageCultivationResinWorldBoss,
                TotalResin = normalBossResin,
                Runs = normalBossRuns,
                Days = (uint)Math.Ceiling((double)normalBossResin / DailyResin),
            });
        }

        if (weeklyBossResin > 0)
        {
            result.Add(new()
            {
                Category = SH.ViewPageCultivationResinWeeklyBoss,
                TotalResin = weeklyBossResin,
                Runs = weeklyBossRuns,
                Days = (uint)Math.Ceiling((double)weeklyBossResin / DailyResin),
            });
        }

        return result;
    }

    private static double QualityMultiplier(QualityType rank)
    {
        return rank switch
        {
            QualityType.QUALITY_GREEN => 1,
            QualityType.QUALITY_BLUE => 3,
            QualityType.QUALITY_PURPLE => 9,
            QualityType.QUALITY_ORANGE => 27,
            _ => 1,
        };
    }

    private static MaterialDropDistribution GetDistribution(int worldLevel)
    {
        return worldLevel switch
        {
            9 => MaterialDropDistribution.Nine,
            8 => MaterialDropDistribution.Eight,
            7 => MaterialDropDistribution.Seven,
            6 => MaterialDropDistribution.Six,
            _ => MaterialDropDistribution.Five,
        };
    }
}

internal sealed class ResinEstimationItem
{
    public required string Category { get; init; }

    public required uint TotalResin { get; init; }

    public required uint Runs { get; init; }

    public required uint Days { get; init; }

    public uint FragileResin => (uint)Math.Ceiling(TotalResin / 60.0);

    public string DaysText => SH.FormatViewModelCultivationResinStatisticsItemRemainDays(Days);
}
