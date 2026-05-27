//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Primitive;

namespace kyxsan.Model.Metadata;

internal sealed class Chapter
{
    public required ChapterId Id { get; init; }

    public required ChapterGroupId GroupId { get; init; }

    public required QuestId BeginQuestId { get; init; }

    public required QuestId EndQuestId { get; init; }

    public required uint NeedPlayerLevel { get; init; }

    public string? Number { get; init; }

    public string? Title { get; init; }

    public required string Icon { get; init; }

    public string? ImageTitle { get; init; }

    public required string SerialNumberIcon { get; init; }

    public required City CityId { get; init; }

    public required QuestType QuestType { get; init; }
}
