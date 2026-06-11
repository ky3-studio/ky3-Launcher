//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.AvatarProperty;

[SuppressMessage("", "SA1202")]
internal static class AvatarPropertySortDescriptions
{
    private static readonly SortDescription LevelNumber = new(nameof(CharacterView.LevelNumber), SortDirection.Descending);
    private static readonly SortDescription Quality = new(nameof(CharacterView.Quality), SortDirection.Descending);
    private static readonly SortDescription Element = new(nameof(CharacterView.Element), SortDirection.Descending);
    private static readonly SortDescription Id = new(nameof(CharacterView.Id), SortDirection.Descending);
    private static readonly SortDescription ActivatedConstellationCount = new(nameof(CharacterView.ActivatedConstellationCount), SortDirection.Descending);
    private static readonly SortDescription FetterLevel = new(nameof(CharacterView.FetterLevel), SortDirection.Descending);

    private static readonly ImmutableArray<SortDescription> DefaultSortDescriptions = [];

    private static readonly ImmutableArray<SortDescription> LevelNumberSortDescriptions =
    [
        LevelNumber,
        Quality,
        Element,
        Id,
    ];

    private static readonly ImmutableArray<SortDescription> QualitySortDescriptions =
    [
        Quality,
        LevelNumber,
        Element,
        Id,
    ];

    private static readonly ImmutableArray<SortDescription> ActivatedConstellationCountSortDescriptions =
    [
        ActivatedConstellationCount,
        LevelNumber,
        Quality,
        Element,
        Id,
    ];

    private static readonly ImmutableArray<SortDescription> FetterLevelSortDescriptions =
    [
        FetterLevel,
        LevelNumber,
        Quality,
        Element,
        Id,
    ];

    public static ImmutableArray<SortDescription> Get(AvatarPropertySortDescriptionKind kind)
    {
        return kind switch
        {
            AvatarPropertySortDescriptionKind.LevelNumber => LevelNumberSortDescriptions,
            AvatarPropertySortDescriptionKind.Quality => QualitySortDescriptions,
            AvatarPropertySortDescriptionKind.ActivatedConstellationCount => ActivatedConstellationCountSortDescriptions,
            AvatarPropertySortDescriptionKind.FetterLevel => FetterLevelSortDescriptions,
            _ => DefaultSortDescriptions,
        };
    }
}
