//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic.Frozen;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using System.Globalization;

namespace kyxsan.ViewModel.Wiki;

// ReSharper disable PossibleMultipleEnumeration
internal static class AvatarFilter
{
    public static Predicate<Avatar>? Compile(SearchData? searchData)
    {
        return searchData is { FilterTokens.Count: > 0 } ? Compile(searchData.FilterTokens) : default;
    }

    public static Predicate<Avatar> Compile(IEnumerable<SearchToken> input)
    {
        ILookup<SearchTokenKind, string> lookup = input.ToLookup(token => token.Kind, token => token.Value);
        return avatar => Compile(lookup, avatar);
    }

    private static bool Compile(ILookup<SearchTokenKind, string> lookup, Avatar avatar)
    {
        List<bool> matches = [];

        // Tokens is a BCL internal Grouping<string>, enumerating will use an internal PartialArrayEnumerator<string>
        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in lookup)
        {
            switch (kind)
            {
                case SearchTokenKind.ElementName:
                    if (IntrinsicFrozen.ElementNames.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.FetterInfo.VisionBefore));
                    }

                    break;
                case SearchTokenKind.AssociationType:
                    if (IntrinsicFrozen.AssociationTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.FetterInfo.Association.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Weapon.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Quality.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.BodyType:
                    if (IntrinsicFrozen.BodyTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Body.GetLocalizedDescriptionOrDefault(SH.ResourceManager)));
                    }

                    break;
                case SearchTokenKind.Avatar:
                    matches.Add(tokens.Contains(avatar.Name));
                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.All(r => r);
    }
}