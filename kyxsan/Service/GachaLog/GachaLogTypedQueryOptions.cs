//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Service.GachaLog.QueryProvider;
using System.Collections.Specialized;
using System.Web;

namespace kyxsan.Service.GachaLog;

internal sealed class GachaLogTypedQueryOptions
{
    public const int Size = 20;

    private readonly NameValueCollection innerQuery;

    public GachaLogTypedQueryOptions(in GachaLogQuery query, GachaType queryType)
    {
        IsOversea = query.IsOversea;
        Type = queryType;
        innerQuery = HttpUtility.ParseQueryString(query.Query);
        innerQuery.Set("gacha_type", $"{queryType:D}");
        innerQuery.Set("size", $"{Size}");
    }

    public bool IsOversea { get; }

    public GachaType Type { get; }

    public long EndId { get; set; }

    public string ToQueryString()
    {
        innerQuery.Set("end_id", $"{EndId:D}");
        string? query = innerQuery.ToString();
        ArgumentException.ThrowIfNullOrEmpty(query);
        return query;
    }
}
