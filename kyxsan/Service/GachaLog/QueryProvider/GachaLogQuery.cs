//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Service.GachaLog.QueryProvider;

internal readonly struct GachaLogQuery
{
    public readonly string Message;
    public readonly string Query;
    public readonly bool IsOversea;

    public GachaLogQuery(string query)
    {
        Query = query;
        IsOversea = query.Contains("region=os_", StringComparison.OrdinalIgnoreCase);
        Message = string.Empty;
    }

    private GachaLogQuery(string query, string message)
    {
        Query = query;
        Message = message;
    }

    public bool IsInvalid { get => string.IsNullOrEmpty(Message); }

    public static GachaLogQuery Invalid(string message)
    {
        return new(string.Empty, message);
    }
}
