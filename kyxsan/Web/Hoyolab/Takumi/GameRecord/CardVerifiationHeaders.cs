//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Endpoint.Hoyolab;

namespace kyxsan.Web.Hoyolab.Takumi.GameRecord;

internal sealed class CardVerifiationHeaders
{
    public int ChallengeGame { get; private set; }

    public string ChallengePath { get; private set; } = string.Empty;

    public string Page { get; private set; } = string.Empty;

    public static CardVerifiationHeaders CreateForDailyNote(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordDailyNotePath());
    }

    public static CardVerifiationHeaders CreateForIndex(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordIndexPath());
    }

    public static CardVerifiationHeaders CreateForSpiralAbyss(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordSpiralAbyssPath());
    }

    public static CardVerifiationHeaders CreateForCharacterAll(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordCharacterList(), $"{HoyolabOptions.ToolVersion}_#/ys/role/all");
    }

    public static CardVerifiationHeaders CreateForCharacterDetail(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordCharacterList(), $"{HoyolabOptions.ToolVersion}_#/ys/role/detail");
    }

    public static CardVerifiationHeaders CreateForRoleCombat(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordRoleCombatPath());
    }

    public static CardVerifiationHeaders CreateForHardChallenge(IApiEndpoints apiEndpoints)
    {
        return Create(apiEndpoints.GameRecordHardChallengePath());
    }

    private static CardVerifiationHeaders Create(string path, string page = $"{HoyolabOptions.ToolVersion}_#/ys")
    {
        return new()
        {
            ChallengeGame = 2,
            ChallengePath = path,
            Page = page,
        };
    }
}