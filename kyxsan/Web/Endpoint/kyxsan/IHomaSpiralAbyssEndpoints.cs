//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Endpoint.kyxsan;

internal interface IHomaSpiralAbyssEndpoints : IHomaRootAccess
{
    string RecordCheck(string uid)
    {
        return $"{Root}/Record/Check?Uid={uid}";
    }

    string RecordRank(string uid)
    {
        return $"{Root}/Record/Rank?Uid={uid}";
    }

    string RecordUpload()
    {
        return $"{Root}/Record/Upload";
    }

    string StatisticsOverview(bool last = false)
    {
        return $"{Root}/Statistics/Overview?Last={last}";
    }

    string StatisticsAvatarAttendanceRate(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/AttendanceRate?Last={last}";
    }

    string StatisticsAvatarUtilizationRate(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/UtilizationRate?Last={last}";
    }

    string StatisticsAvatarAvatarCollocation(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/AvatarCollocation?Last={last}";
    }

    string StatisticsAvatarHoldingRate(bool last = false)
    {
        return $"{Root}/Statistics/Avatar/HoldingRate?Last={last}";
    }

    string StatisticsWeaponWeaponCollocation(bool last = false)
    {
        return $"{Root}/Statistics/Weapon/WeaponCollocation?Last={last}";
    }

    string StatisticsTeamCombination(bool last = false)
    {
        return $"{Root}/Statistics/Team/Combination?Last={last}";
    }
}