//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;

namespace kyxsan.Web.Hoyolab;

internal readonly struct PlayerUid
{
    public readonly string Value;

    public readonly Region Region;

    public PlayerUid(string value, in Region? region = default)
    {
        kyxsanException.ThrowIfNot(HoyolabRegex.UidRegex.IsMatch(value), SH.WebHoyolabInvalidUid);
        Value = value;
        Region = region ?? Region.UnsafeFromUidString(value);
    }

    public static implicit operator PlayerUid(string source)
    {
        return FromUidString(source);
    }

    public static PlayerUid FromUidString(string uid)
    {
        return new(uid);
    }

    public static bool IsOversea(string uid)
    {
        kyxsanException.ThrowIfNot(HoyolabRegex.UidRegex.IsMatch(uid), SH.WebHoyolabInvalidUid);

        return uid.AsSpan()[^9] switch
        {
            >= '1' and <= '5' => false,
            _ => true,
        };
    }

    public static TimeSpan GetRegionTimeZoneUtcOffsetForUid(string uid)
    {
        kyxsanException.ThrowIfNot(HoyolabRegex.UidRegex.IsMatch(uid), SH.WebHoyolabInvalidUid);

        // 美服 UTC-05
        // 欧服 UTC+01
        // 其他 UTC+08
        return uid.AsSpan()[^9] switch
        {
            '6' => ServerRegionTimeZone.AmericaServerOffset,
            '7' => ServerRegionTimeZone.EuropeServerOffset,
            _ => ServerRegionTimeZone.CommonOffset,
        };
    }

    public static TimeSpan GetRegionTimeZoneUtcOffsetForRegion(in Region region)
    {
        // 美服 UTC-05
        // 欧服 UTC+01
        // 其他 UTC+08
        return region.Value switch
        {
            "os_usa" => ServerRegionTimeZone.AmericaServerOffset,
            "os_euro" => ServerRegionTimeZone.EuropeServerOffset,
            _ => ServerRegionTimeZone.CommonOffset,
        };
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
}