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

[JsonConverter(typeof(RegionConverter))]
internal readonly struct Region : IEquatable<Region>
{
    public static readonly Region CNGF01 = new("cn_gf01");
    public static readonly Region CNQD01 = new("cn_qd01");
    public static readonly Region OSUSA = new("os_usa");
    public static readonly Region OSEURO = new("os_euro");
    public static readonly Region OSASIA = new("os_asia");
    public static readonly Region OSCHT = new("os_cht");

    public readonly string Value;

    public Region(string value)
    {
        kyxsanException.ThrowIfNot(HoyolabRegex.RegionRegex.IsMatch(value), SH.WebHoyolabInvalidRegion);
        Value = value;
    }

    public static implicit operator Region(string value)
    {
        return FromRegionString(value);
    }

    public static Region FromRegionString(string value)
    {
        return new(value);
    }

    public static string ToRegionString(Region region)
    {
        return region.Value;
    }

    public static Region UnsafeFromUidString(string uid)
    {
        return uid.AsSpan()[^9] switch
        {
            // CN
            >= '1' and <= '4' => CNGF01, // 国服
            '5' => CNQD01,               // 渠道

            // OS
            '6' => OSUSA,  // 美服
            '7' => OSEURO, // 欧服
            '8' => OSASIA, // 亚服
            '9' => OSCHT,  // 台服
            _ => throw kyxsanException.NotSupported(),
        };
    }

    public static bool IsOversea(string value)
    {
        kyxsanException.ThrowIfNot(HoyolabRegex.RegionRegex.IsMatch(value), SH.WebHoyolabInvalidRegion);
        return value.AsSpan()[..2] switch
        {
            "os" => true,
            _ => false,
        };
    }

    public bool IsOversea()
    {
        return IsOversea(Value);
    }

    public override string ToString()
    {
        return Value;
    }

    public bool Equals(Region other)
    {
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Region other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}