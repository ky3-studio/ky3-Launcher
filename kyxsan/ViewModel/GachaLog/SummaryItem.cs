//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Windows.UI;

namespace kyxsan.ViewModel.GachaLog;

internal sealed class SummaryItem : Model.Item
{
    public bool IsUp { get; set; }

    public bool IsNotUp { get; set; }

    public bool IsGuarantee { get; set; }

    public int GuaranteeOrangeThreshold { get; set; }

    public int LastPull { get; set; }

    public string FormattedTime
    {
        get => $"{Time.ToLocalTime():yyy.MM.dd HH:mm:ss}";
    }

    public Color Color { get; set; }

    internal DateTimeOffset Time { get; set; }
}
