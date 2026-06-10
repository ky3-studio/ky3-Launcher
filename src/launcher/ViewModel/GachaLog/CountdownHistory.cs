//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata;

namespace kyxsan.ViewModel.GachaLog;

internal sealed class CountdownHistory
{
    public CountdownHistory(GachaEvent gachaEvent)
    {
        LastTime = gachaEvent.To;
        FormattedTime = $"{gachaEvent.To:yyyy-MM-dd}";
        FormattedVersionOrder = $"{gachaEvent.Version} {(gachaEvent.Order is 1 ? SH.ViewModelGachaLogCountdownOrderUp : SH.ViewModelGachaLogCountdownOrderDown)}";
        Banner = gachaEvent.Banner;
    }

    public string FormattedTime { get; }

    public string FormattedVersionOrder { get; }

    public Uri Banner { get; }

    internal DateTimeOffset LastTime { get; }
}
