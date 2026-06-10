//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic.Frozen;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using System.Globalization;

namespace kyxsan.ViewModel.Wiki;

// ReSharper disable PossibleMultipleEnumeration
internal static class WeaponFilter
{
    public static Predicate<Weapon>? Compile(SearchData? searchData)
    {
        return searchData is { FilterTokens.Count: > 0 } ? Compile(searchData.FilterTokens) : default;
    }

    public static Predicate<Weapon> Compile(IEnumerable<SearchToken> input)
    {
        ILookup<SearchTokenKind, string> lookup = input.ToLookup(token => token.Kind, token => token.Value);
        return weapon => DoFilter(lookup, weapon);
    }

    private static bool DoFilter(ILookup<SearchTokenKind, string> lookup, Weapon weapon)
    {
        List<bool> matches = [];

        // Tokens is a BCL internal Grouping<string>, enumerating will use an internal PartialArrayEnumerator<string>
        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in lookup)
        {
            switch (kind)
            {
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(weapon.WeaponType.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(weapon.Quality.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.FightProperty:
                    if (IntrinsicFrozen.FightProperties.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(weapon.GrowCurves.Array.ElementAtOrDefault(1)?.Type.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.Weapon:
                    matches.Add(tokens.Contains(weapon.Name));
                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.All(r => r);
    }
}