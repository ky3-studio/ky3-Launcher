//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace kyxsan.Web.kyxsan;

internal sealed partial class IPInformation
{
    private const string Unknown = "Unknown";

    public static IPInformation Default { get; } = new()
    {
        Ip = Unknown,
        Division = Unknown,
    };

    [JsonPropertyName("ip")]
    public required string Ip { get; init; }

    [JsonPropertyName("division")]
    public required string Division { get; init; }

    [GeneratedRegex(@"(\d+)\.(\d+)\.\d+\.\d+")]
    private static partial Regex IpRegex { get; }

    public override string ToString()
    {
        if (Ip is Unknown && Division is Unknown)
        {
            return SH.WebkyxsanServiceUnAvailable;
        }

        string maskedIp = IpRegex.Replace(Ip, "$1.$2.*.*");
        return SH.FormatViewPageSettingDeviceIpDescription(maskedIp, Division);
    }
}