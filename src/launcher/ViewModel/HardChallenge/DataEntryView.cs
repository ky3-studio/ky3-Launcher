//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.UI.Xaml.Data;
using Launcher.UI.Xaml.Data.Converter.Specialized;
using Launcher.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

namespace Launcher.ViewModel.HardChallenge;

internal sealed partial class DataEntryView : IPropertyValuesProvider
{
    private DataEntryView(bool singlePlayer, HardChallengeDataEntry dataEntry, HardChallengeMetadataContext context)
    {
        Name = singlePlayer
            ? SH.ViewModelHardChalllengeDataEntrySinglePlayerName
            : SH.ViewModelHardChalllengeDataEntryMultiPlayerName;
        Debug.Assert(dataEntry.HasData);
        DifficultyIcon = HardChallengeDifficultyIconConverter.Convert(dataEntry.Best.Icon.Split(',').Last());
        Difficulty = dataEntry.Best.Difficulty.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
        FormattedSeconds = SH.FormatViewModelHardChallengeSeconds(dataEntry.Best.Seconds);

        Challenges = dataEntry.Challenges.SelectAsArray(ChallengeView.Create, context);
    }

    public string Name { get; }

    public Uri DifficultyIcon { get; }

    public string? Difficulty { get; }

    public string FormattedSeconds { get; }

    public ImmutableArray<ChallengeView> Challenges { get; }

    public static DataEntryView? Create(bool singlePlayer, HardChallengeDataEntry dataEntry, HardChallengeMetadataContext context)
    {
        if (!dataEntry.HasData)
        {
            return default;
        }

        return new(singlePlayer, dataEntry, context);
    }
}