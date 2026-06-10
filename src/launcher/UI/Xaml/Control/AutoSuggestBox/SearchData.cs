//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace kyxsan.UI.Xaml.Control.AutoSuggestBox;

internal sealed partial class SearchData : ObservableObject
{
    private SearchData(FrozenDictionary<string, SearchToken> availableTokens)
    {
        AvailableTokens = availableTokens;
    }

    public FrozenDictionary<string, SearchToken> AvailableTokens { get; }

    public ObservableCollection<SearchToken> FilterTokens { get; } = [];

    [ObservableProperty]
    public partial string? FilterToken { get; set; }

    public static SearchData CreateForAvatarProperty()
    {
        return new(SearchTokens.GetForAvatarProperty());
    }

    public static SearchData CreateForCultivation()
    {
        return new(SearchTokens.GetForCultivation());
    }

    public static SearchData CreateForWikiAvatar(ImmutableArray<Avatar> array)
    {
        return new(SearchTokens.GetForWikiAvatar(array));
    }

    public static SearchData CreateForWikiWeapon(ImmutableArray<Weapon> array)
    {
        return new(SearchTokens.GetForWikiWeapon(array));
    }
}