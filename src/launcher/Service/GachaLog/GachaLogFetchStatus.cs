//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Intrinsic;
using System.Globalization;

namespace kyxsan.Service.GachaLog;

internal sealed class GachaLogFetchStatus
{
    public GachaLogFetchStatus(GachaType configType)
    {
        ConfigType = configType;
    }

    public bool AuthKeyTimeout { get; set; }

    public GachaType ConfigType { get; }

    public List<Item> Items { get; } = new(GachaLogTypedQueryOptions.Size);

    public string Header
    {
        get
        {
            return AuthKeyTimeout
                ? SH.ViewDialogGachaLogRefreshProgressAuthkeyTimeout
                : SH.FormatViewDialogGachaLogRefreshProgressDescription(ConfigType.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture));
        }
    }
}
