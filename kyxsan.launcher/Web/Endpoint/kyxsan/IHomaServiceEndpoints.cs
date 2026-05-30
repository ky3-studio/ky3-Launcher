//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Endpoint.kyxsan;

internal interface IHomaServiceEndpoints : IHomaRootAccess
{
    string AnnouncementUpload()
    {
        return $"{Root}/Service/Announcement/Upload";
    }

    string CdnCompensation(int days)
    {
        return $"{Root}/Service/Distribution/Compensation?days={days}";
    }

    string CdnDesignation(string userName, int days)
    {
        return $"{Root}/Service/Distribution/Designation?userName={userName}&days={days}";
    }

    string RedeemCodeGenerate()
    {
        return $"{Root}/Service/Redeem/Generate";
    }
}