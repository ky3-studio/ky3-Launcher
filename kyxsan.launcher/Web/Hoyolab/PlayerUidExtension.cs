//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Specialized;
using System.Web;

namespace kyxsan.Web.Hoyolab;

internal static class PlayerUidExtension
{
    // IMPORTANT: DO NOT CONVERT TO EXTENSION BLOCK
    // ReSharper disable once ConvertToExtensionBlock
    public static string ToRoleIdServerQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
        collection.Set("role_id", playerUid.Value);
        collection.Set("server", playerUid.Region.Value);

        string? query = collection.ToString();
        ArgumentNullException.ThrowIfNull(query);
        return query;
    }

    // IMPORTANT: DO NOT CONVERT TO EXTENSION BLOCK
    // ReSharper disable once ConvertToExtensionBlock
    public static string ToUidRegionQueryString(this in PlayerUid playerUid)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
        collection.Set("uid", playerUid.Value);
        collection.Set("region", playerUid.Region.Value);

        string? query = collection.ToString();
        ArgumentNullException.ThrowIfNull(query);
        return query;
    }
}