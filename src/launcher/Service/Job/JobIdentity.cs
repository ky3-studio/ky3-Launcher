//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Service.Job;

internal static class JobIdentity
{
    public const string DailyNoteGroupName = "DailyNote";
    public const string DailyNoteRefreshJobName = "RefreshJob";
    public const string DailyNoteRefreshTriggerName = "RefreshTrigger";

    public const string CookieTokenGroupName = "CookieToken";
    public const string CookieTokenRefreshJobName = "RefreshJob";
    public const string CookieTokenRefreshTriggerName = "RefreshTrigger";
}