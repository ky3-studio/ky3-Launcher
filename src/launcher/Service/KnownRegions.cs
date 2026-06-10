//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Web.Hoyolab;
using System.Collections.Immutable;
using System.Diagnostics;

namespace kyxsan.Service;

internal static class KnownRegions
{
    public static ImmutableArray<NameValue<Region>> Value
    {
        get
        {
            Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
            return !field.IsDefault ? field : field =
            [
                new(SH.WebHoyolabRegionCNGF01, Region.CNGF01),
                new(SH.WebHoyolabRegionCNQD01, Region.CNQD01),
                new(SH.WebHoyolabRegionOSUSA, Region.OSUSA),
                new(SH.WebHoyolabRegionOSEURO, Region.OSEURO),
                new(SH.WebHoyolabRegionOSASIA, Region.OSASIA),
                new(SH.WebHoyolabRegionOSCHT, Region.OSCHT),
            ];
        }
    }
}