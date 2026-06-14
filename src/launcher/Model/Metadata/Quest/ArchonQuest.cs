//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Quest;

internal sealed partial class ArchonQuest : IPropertyValuesProvider
{
    public required uint Id { get; init; }

    public required string Type { get; init; }

    public required string TypeName { get; init; }

    public required string ChapterNum { get; init; }

    public required string ChapterTitle { get; init; }

    public required string ChapterIcon { get; init; }

    public required string Region { get; init; }

    public required uint StoryCount { get; init; }

    public required ImmutableArray<ArchonQuestStory> Stories { get; init; }
}
