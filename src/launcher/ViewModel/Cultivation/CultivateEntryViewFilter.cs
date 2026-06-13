// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity.Primitive;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Intrinsic.Frozen;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Model.Primitive;
using kyxsan.Service.Cultivation;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using System.Collections.ObjectModel;
using System.Globalization;

namespace kyxsan.ViewModel.Cultivation;

internal static class CultivateEntryViewFilter
{
    public static Predicate<CultivateEntryView>? Compile(SearchData? searchData, ICultivationMetadataContext metadataContext)
    {
        return searchData is { FilterTokens.Count: > 0 } ? Compile(searchData.FilterTokens, metadataContext) : default;
    }

    public static Predicate<CultivateEntryView> Compile(ObservableCollection<SearchToken> input, ICultivationMetadataContext metadataContext)
    {
        return entry => DoFilter(input, entry, metadataContext);
    }

    private static bool DoFilter(ObservableCollection<SearchToken> input, CultivateEntryView entry, ICultivationMetadataContext metadataContext)
    {
        List<bool> matches = [];

        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in input.GroupBy(token => token.Kind, token => token.Value))
        {
            switch (kind)
            {
                case SearchTokenKind.ElementName:
                    if (entry.Type is CultivateType.AvatarAndSkill && IntrinsicFrozen.ElementNames.Overlaps(tokens))
                    {
                        if (metadataContext.IdAvatarMap.TryGetValue((AvatarId)entry.Id, out Avatar? avatar))
                        {
                            matches.Add(tokens.Contains(avatar.FetterInfo.VisionBefore));
                        }
                    }

                    break;
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        WeaponType weaponType = entry.Type switch
                        {
                            CultivateType.AvatarAndSkill => metadataContext.IdAvatarMap.TryGetValue((AvatarId)entry.Id, out Avatar? avatar) ? avatar.Weapon : WeaponType.WEAPON_NONE,
                            CultivateType.Weapon => metadataContext.IdWeaponMap.TryGetValue((WeaponId)entry.Id, out Weapon? weapon) ? weapon.WeaponType : WeaponType.WEAPON_NONE,
                            _ => WeaponType.WEAPON_NONE,
                        };

                        matches.Add(tokens.Contains(weaponType.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(entry.Quality.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.CultivateType:
                    if (IntrinsicFrozen.CultivateTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(entry.Type.GetLocalizedDescriptionOrDefault(SH.ResourceManager)));
                    }

                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}
