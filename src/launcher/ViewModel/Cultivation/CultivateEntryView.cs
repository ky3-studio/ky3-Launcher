// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Entity;
using kyxsan.Model.Entity.Primitive;
using kyxsan.Model.Intrinsic;
using kyxsan.Service.Cultivation;
using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Text;

namespace kyxsan.ViewModel.Cultivation;

internal sealed partial class CultivateEntryView : IPropertyValuesProvider
{
    public required Guid EntryId { get; init; }

    public required CultivateType Type { get; init; }

    public required uint Id { get; init; }

    public required string Name { get; init; }

    public required string Icon { get; init; }

    public required ImmutableArray<CultivateItemView> Items { get; init; }

    public required string? LevelDescription { get; init; }

    public required QualityType Quality { get; init; }

    public required bool IsToday { get; init; }

    public static CultivateEntryView Create(CultivateEntry entry, ImmutableArray<CultivateItemView> items, ICultivationMetadataContext context, TimeSpan offset)
    {
        Item item = entry.Type switch
        {
            CultivateType.AvatarAndSkill => context.GetAvatarItem(entry.Id),
            CultivateType.Weapon => context.GetWeaponItem(entry.Id),
            _ => new() { Name = $"??? ({entry.Id})", Icon = default!, Badge = default!, Quality = default },
        };

        string? levelDesc = ParseDescription(entry);
        bool isToday = items.Any(i => i.IsToday);

        return new()
        {
            EntryId = entry.InnerId,
            Type = entry.Type,
            Id = entry.Id,
            Name = item.Name,
            Icon = item.Icon?.ToString() ?? string.Empty,
            Quality = item.Quality,
            Items = items,
            LevelDescription = levelDesc,
            IsToday = isToday,
        };
    }

    private static string? ParseDescription(CultivateEntry entry)
    {
        if (entry.LevelInformation is not { } info)
        {
            return null;
        }

        switch (entry.Type)
        {
            case CultivateType.AvatarAndSkill:
                {
                    StringBuilder sb = new();
                    if (info.AvatarLevelFrom != info.AvatarLevelTo)
                    {
                        sb.Append("Lv.").Append(info.AvatarLevelFrom).Append(" \u2192 Lv.").Append(info.AvatarLevelTo);
                        sb.AppendLine();
                    }

                    if (info.SkillALevelFrom != info.SkillALevelTo)
                    {
                        sb.Append("A: ").Append(info.SkillALevelFrom).Append(" \u2192 ").Append(info.SkillALevelTo).Append(' ');
                    }

                    if (info.SkillELevelFrom != info.SkillELevelTo)
                    {
                        sb.Append("E: ").Append(info.SkillELevelFrom).Append(" \u2192 ").Append(info.SkillELevelTo).Append(' ');
                    }

                    if (info.SkillQLevelFrom != info.SkillQLevelTo)
                    {
                        sb.Append("Q: ").Append(info.SkillQLevelFrom).Append(" \u2192 ").Append(info.SkillQLevelTo).Append(' ');
                    }

                    return sb.ToString().TrimEnd();
                }

            case CultivateType.Weapon:
                {
                    if (info.WeaponLevelFrom != info.WeaponLevelTo)
                    {
                        return $"Lv.{info.WeaponLevelFrom} \u2192 Lv.{info.WeaponLevelTo}";
                    }

                    break;
                }
        }

        return null;
    }
}
