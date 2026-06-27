// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Weapon;
using Launcher.Model.Primitive;
using Launcher.UI.Xaml.View.Dialog;
using System.Collections.Immutable;

namespace Launcher.Service.Cultivation.Offline;

internal static class OfflineCalculator
{
    private static ReadOnlySpan<uint> AscensionLevelThresholds => [20, 40, 50, 60, 70, 80];

    private static ReadOnlySpan<uint> AscensionMoraCost => [20000, 40000, 60000, 80000, 100000, 120000];

    // Quality offsets from highest: Sliver=-3, Fragment=-2, Chunk=-1, Gemstone=0
    private static ReadOnlySpan<uint> AscensionGemCounts => [1, 3, 6, 3, 6, 6];
    private static ReadOnlySpan<int> AscensionGemQualityOffsets => [-3, -2, -2, -1, -1, 0];

    private static ReadOnlySpan<uint> AscensionLocalSpecialtyCounts => [3, 10, 20, 30, 45, 60];

    private static ReadOnlySpan<uint> AscensionBossDropCounts => [0, 2, 4, 8, 12, 20];

    // Quality offsets: Green=-2, Blue=-1, Purple=0
    private static ReadOnlySpan<uint> AscensionCommonCounts => [3, 15, 12, 18, 12, 24];
    private static ReadOnlySpan<int> AscensionCommonQualityOffsets => [-2, -2, -1, -1, 0, 0];

    // Talent level-up costs (index = target level - 2, i.e. index 0 = 1¡ú2)
    private static ReadOnlySpan<uint> TalentBookCounts => [3, 2, 4, 6, 9, 4, 6, 12, 16];
    private static ReadOnlySpan<int> TalentBookQualityOffsets => [-2, -1, -1, -1, -1, 0, 0, 0, 0];

    private static ReadOnlySpan<uint> TalentCommonCounts => [6, 3, 4, 6, 9, 4, 6, 9, 12];
    private static ReadOnlySpan<int> TalentCommonQualityOffsets => [-2, -1, -1, -1, -1, 0, 0, 0, 0];

    private static ReadOnlySpan<uint> TalentMoraCosts => [12500, 17500, 25000, 30000, 37500, 120000, 260000, 450000, 700000];

    private static ReadOnlySpan<uint> TalentWeeklyBossCounts => [0, 0, 0, 0, 0, 1, 1, 2, 2];

    private const uint CrownOfInsightId = 104319;
    private static ReadOnlySpan<uint> TalentCrownCounts => [0, 0, 0, 0, 0, 0, 0, 0, 1];

    private const uint HerosWitId = 104003;
    private const int ExpPerBook = 20000;
    private const uint MoraId = 202;

    private const uint MysticEnhancementOreId = 104013;
    private const int WeaponOreExp = 10000;
    private const uint WeaponOreUseMora = 1000;

    // Weapon ascension thresholds (same as avatar)
    private static ReadOnlySpan<uint> WeaponAscensionThresholds => [20, 40, 50, 60, 70, 80];

    // Weapon ascension mora costs by quality [quality][phase]
    // Index: 0=None, 1=White, 2=Green, 3=Blue, 4=Purple, 5=Orange
    private static ReadOnlySpan<uint> WeaponAscMoraBlue => [5000, 10000, 15000, 20000, 25000, 30000];
    private static ReadOnlySpan<uint> WeaponAscMoraPurple => [5000, 15000, 20000, 30000, 35000, 45000];
    private static ReadOnlySpan<uint> WeaponAscMoraOrange => [10000, 20000, 30000, 45000, 55000, 65000];

    // Weapon domain material counts [quality][phase] as (qualityOffset, count)
    // CultivationItems[0] = highest quality weapon domain material
    private static ReadOnlySpan<uint> WeaponMatCountsBlue => [2, 2, 4, 2, 4, 3]; // offset pattern: 3,2,2,1,1,0
    private static ReadOnlySpan<int> WeaponMatOffsetsBlue => [-3, -2, -2, -1, -1, 0];
    private static ReadOnlySpan<uint> WeaponMatCountsPurple => [3, 3, 6, 3, 6, 4];
    private static ReadOnlySpan<int> WeaponMatOffsetsPurple => [-3, -2, -2, -1, -1, 0];
    private static ReadOnlySpan<uint> WeaponMatCountsOrange => [5, 5, 9, 5, 9, 6];
    private static ReadOnlySpan<int> WeaponMatOffsetsOrange => [-3, -2, -2, -1, -1, 0];

    // Weapon elite monster material A counts
    private static ReadOnlySpan<uint> WeaponMonACountsBlue => [2, 8, 4, 8, 6, 12];
    private static ReadOnlySpan<int> WeaponMonAOffsetsBlue => [-2, -2, -1, -1, 0, 0];
    private static ReadOnlySpan<uint> WeaponMonACountsPurple => [3, 12, 6, 12, 9, 18];
    private static ReadOnlySpan<int> WeaponMonAOffsetsPurple => [-2, -2, -1, -1, 0, 0];
    private static ReadOnlySpan<uint> WeaponMonACountsOrange => [5, 18, 9, 18, 14, 27];
    private static ReadOnlySpan<int> WeaponMonAOffsetsOrange => [-2, -2, -1, -1, 0, 0];

    // Weapon common monster material B counts
    private static ReadOnlySpan<uint> WeaponMonBCountsBlue => [1, 5, 4, 6, 4, 8];
    private static ReadOnlySpan<int> WeaponMonBOffsetsBlue => [-2, -2, -1, -1, 0, 0];
    private static ReadOnlySpan<uint> WeaponMonBCountsPurple => [2, 8, 6, 9, 6, 12];
    private static ReadOnlySpan<int> WeaponMonBOffsetsPurple => [-2, -2, -1, -1, 0, 0];
    private static ReadOnlySpan<uint> WeaponMonBCountsOrange => [3, 12, 9, 14, 9, 18];
    private static ReadOnlySpan<int> WeaponMonBOffsetsOrange => [-2, -2, -1, -1, 0, 0];

    /// <summary>
    /// Avatar CultivationItems layout:
    /// [0]=Gem(highest) [1]=BossDrop [2]=LocalSpecialty [3]=Common(highest) [4]=TalentBook(highest) [5]=WeeklyBoss
    /// </summary>
    public static Dictionary<uint, uint> CalculateAvatarMaterials(Avatar avatar, CultivateLevelInput input)
    {
        Dictionary<uint, uint> materials = [];

        // Avatar's CultivationItems layout (6 items):
        // [0] = Gem (highest quality, e.g., Gemstone)
        // [1] = Boss Drop
        // [2] = Local Specialty
        // [3] = Common Enemy Material (highest quality, e.g., Purple)
        // [4] = Talent Book (highest quality, e.g., Gold/Philosophies)
        // [5] = Weekly Boss Material
        ImmutableArray<MaterialId> items = avatar.CultivationItems;
        if (items.Length < 6)
        {
            return materials;
        }

        uint gemBaseId = (uint)items[0];
        uint bossDropId = (uint)items[1];
        uint localSpecialtyId = (uint)items[2];
        uint commonBaseId = (uint)items[3];
        uint talentBookBaseId = (uint)items[4];
        uint weeklyBossId = (uint)items[5];

        CalculateAscensionMaterials(materials, input.AvatarLevelFrom, input.AvatarLevelTo,
            gemBaseId, bossDropId, localSpecialtyId, commonBaseId);

        CalculateExperienceMaterials(materials, input.AvatarLevelFrom, input.AvatarLevelTo);

        CalculateTalentMaterials(materials, input.SkillALevelFrom, input.SkillALevelTo,
            talentBookBaseId, commonBaseId, weeklyBossId);
        CalculateTalentMaterials(materials, input.SkillELevelFrom, input.SkillELevelTo,
            talentBookBaseId, commonBaseId, weeklyBossId);
        CalculateTalentMaterials(materials, input.SkillQLevelFrom, input.SkillQLevelTo,
            talentBookBaseId, commonBaseId, weeklyBossId);

        return materials;
    }

    private static void CalculateAscensionMaterials(
        Dictionary<uint, uint> materials,
        uint levelFrom, uint levelTo,
        uint gemBaseId, uint bossDropId, uint localSpecialtyId, uint commonBaseId)
    {
        if (levelFrom >= levelTo)
        {
            return;
        }

        ReadOnlySpan<uint> thresholds = AscensionLevelThresholds;

        for (int phase = 0; phase < 6; phase++)
        {
            uint threshold = thresholds[phase];

            // ascension needed if: levelFrom <= threshold < levelTo
            if (levelFrom <= threshold && threshold < levelTo)
            {
                AddMaterial(materials, MoraId, AscensionMoraCost[phase]);

                uint gemId = (uint)((int)gemBaseId + AscensionGemQualityOffsets[phase]);
                AddMaterial(materials, gemId, AscensionGemCounts[phase]);

                AddMaterial(materials, localSpecialtyId, AscensionLocalSpecialtyCounts[phase]);

                if (AscensionBossDropCounts[phase] > 0)
                {
                    AddMaterial(materials, bossDropId, AscensionBossDropCounts[phase]);
                }

                uint commonId = (uint)((int)commonBaseId + AscensionCommonQualityOffsets[phase]);
                AddMaterial(materials, commonId, AscensionCommonCounts[phase]);
            }
        }
    }

    private static void CalculateExperienceMaterials(
        Dictionary<uint, uint> materials,
        uint levelFrom, uint levelTo)
    {
        int totalExp = AvatarLevelExperience.CalculateTotalExperience((int)levelFrom, (int)levelTo);
        if (totalExp > 0)
        {
            uint booksNeeded = (uint)Math.Ceiling((double)totalExp / ExpPerBook);
            AddMaterial(materials, HerosWitId, booksNeeded);

            // Mora for exp books: 1 Mora per 5 exp = 4000 per Hero's Wit
            uint moraCost = booksNeeded * 4000;
            AddMaterial(materials, MoraId, moraCost);
        }
    }

    private static void CalculateTalentMaterials(
        Dictionary<uint, uint> materials,
        uint levelFrom, uint levelTo,
        uint talentBookBaseId, uint commonBaseId, uint weeklyBossId)
    {
        if (levelFrom >= levelTo || levelFrom < 1 || levelTo > 10)
        {
            return;
        }

        for (uint level = levelFrom; level < levelTo; level++)
        {
            int index = (int)(level - 1);

            AddMaterial(materials, MoraId, TalentMoraCosts[index]);

            uint bookId = (uint)((int)talentBookBaseId + TalentBookQualityOffsets[index]);
            AddMaterial(materials, bookId, TalentBookCounts[index]);

            uint commonId = (uint)((int)commonBaseId + TalentCommonQualityOffsets[index]);
            AddMaterial(materials, commonId, TalentCommonCounts[index]);

            if (TalentWeeklyBossCounts[index] > 0)
            {
                AddMaterial(materials, weeklyBossId, TalentWeeklyBossCounts[index]);
            }

            if (TalentCrownCounts[index] > 0)
            {
                AddMaterial(materials, CrownOfInsightId, TalentCrownCounts[index]);
            }
        }
    }

    private static void AddMaterial(Dictionary<uint, uint> materials, uint materialId, uint count)
    {
        if (materials.TryGetValue(materialId, out uint existing))
        {
            materials[materialId] = existing + count;
        }
        else
        {
            materials[materialId] = count;
        }
    }

    /// <summary>
    /// Weapon CultivationItems layout:
    /// [0]=Domain material(highest) [1]=Elite monster A(highest) [2]=Common monster B(highest)
    /// </summary>
    public static Dictionary<uint, uint> CalculateWeaponMaterials(Weapon weapon, uint levelFrom, uint levelTo, bool ascended = false)
    {
        Dictionary<uint, uint> materials = [];

        ImmutableArray<MaterialId> items = weapon.CultivationItems;
        if (items.Length < 3 || levelFrom >= levelTo)
        {
            return materials;
        }

        uint domainMatBase = (uint)items[0];
        uint monsterABase = (uint)items[1];
        uint monsterBBase = (uint)items[2];
        QualityType quality = weapon.Quality;

        ReadOnlySpan<uint> moraCosts;
        ReadOnlySpan<uint> matCounts;
        ReadOnlySpan<int> matOffsets;
        ReadOnlySpan<uint> monACounts;
        ReadOnlySpan<int> monAOffsets;
        ReadOnlySpan<uint> monBCounts;
        ReadOnlySpan<int> monBOffsets;
        int maxPhase;

        if (quality >= QualityType.QUALITY_ORANGE)
        {
            moraCosts = WeaponAscMoraOrange;
            matCounts = WeaponMatCountsOrange;
            matOffsets = WeaponMatOffsetsOrange;
            monACounts = WeaponMonACountsOrange;
            monAOffsets = WeaponMonAOffsetsOrange;
            monBCounts = WeaponMonBCountsOrange;
            monBOffsets = WeaponMonBOffsetsOrange;
            maxPhase = 6;
        }
        else if (quality == QualityType.QUALITY_PURPLE)
        {
            moraCosts = WeaponAscMoraPurple;
            matCounts = WeaponMatCountsPurple;
            matOffsets = WeaponMatOffsetsPurple;
            monACounts = WeaponMonACountsPurple;
            monAOffsets = WeaponMonAOffsetsPurple;
            monBCounts = WeaponMonBCountsPurple;
            monBOffsets = WeaponMonBOffsetsPurple;
            maxPhase = 6;
        }
        else
        {
            moraCosts = WeaponAscMoraBlue;
            matCounts = WeaponMatCountsBlue;
            matOffsets = WeaponMatOffsetsBlue;
            monACounts = WeaponMonACountsBlue;
            monAOffsets = WeaponMonAOffsetsBlue;
            monBCounts = WeaponMonBCountsBlue;
            monBOffsets = WeaponMonBOffsetsBlue;
            maxPhase = quality >= QualityType.QUALITY_BLUE ? 6 : 4;
        }

        ReadOnlySpan<uint> thresholds = WeaponAscensionThresholds;
        for (int phase = 0; phase < maxPhase; phase++)
        {
            uint threshold = thresholds[phase];
            if ((ascended ? levelFrom < threshold : levelFrom <= threshold) && threshold < levelTo)
            {
                AddMaterial(materials, MoraId, moraCosts[phase]);

                uint matId = (uint)((int)domainMatBase + matOffsets[phase]);
                AddMaterial(materials, matId, matCounts[phase]);

                uint monAId = (uint)((int)monsterABase + monAOffsets[phase]);
                AddMaterial(materials, monAId, monACounts[phase]);

                uint monBId = (uint)((int)monsterBBase + monBOffsets[phase]);
                AddMaterial(materials, monBId, monBCounts[phase]);
            }
        }

        // Calculate weapon EXP (¾«¶ÍÓÃÄ§¿ó)
        int requiredExp = WeaponLevelExperience.CalculateTotalExperience(quality, (int)levelFrom, (int)levelTo);
        if (requiredExp > 0)
        {
            uint oreCount = (uint)Math.Ceiling((double)requiredExp / WeaponOreExp);
            AddMaterial(materials, MysticEnhancementOreId, oreCount);
            AddMaterial(materials, MoraId, oreCount * WeaponOreUseMora);
        }

        return materials;
    }
}
