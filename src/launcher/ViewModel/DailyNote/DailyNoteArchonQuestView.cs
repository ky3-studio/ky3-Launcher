//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata;
using Launcher.Model.Primitive;
using System.Collections.Immutable;
using System.Globalization;
using WebDailyNote = Launcher.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Launcher.ViewModel.DailyNote;

internal sealed class DailyNoteArchonQuestView
{
    private DailyNoteArchonQuestView(WebDailyNote? dailyNote, ImmutableArray<Chapter> chapters)
    {
        Ids = [.. chapters.Where(chapter => chapter.QuestType is QuestType.AQ).Select(chapter => chapter.Id)];

        if (dailyNote is { ArchonQuestProgress.List: [{ } quest, ..] })
        {
            ProgressValue = Ids.IndexOf(quest.Id);
            FormattedProgress = quest.Status.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
            FormattedChapter = $"{quest.ChapterNum} {quest.ChapterTitle}";
        }
        else
        {
            ProgressValue = Ids.Length;
            FormattedProgress = SH.WebDailyNoteArchonQuestStatusFinished;
            FormattedChapter = SH.WebDailyNoteArchonQuestChapterFinished;
        }
    }

    public ImmutableArray<ChapterId> Ids { get; }

    public int ProgressValue { get; }

    public string? FormattedProgress { get; }

    public string FormattedChapter { get; }

    public static DailyNoteArchonQuestView Create(WebDailyNote? dailyNote, ImmutableArray<Chapter> chapters)
    {
        return new(dailyNote, chapters);
    }
}