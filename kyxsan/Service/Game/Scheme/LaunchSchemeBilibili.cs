//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;

namespace kyxsan.Service.Game.Scheme;

internal sealed class LaunchSchemeBilibili : LaunchScheme
{
    private const string HoyoPlayLauncherBilibiliId = "umfgRO5gh5";
    private const string HoyoPlayGameBilibiliId = "T2S0Gz4Dr2";

    public LaunchSchemeBilibili(SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = HoyoPlayLauncherBilibiliId;
        GameId = HoyoPlayGameBilibiliId;
        Channel = ChannelType.Bili;
        SubChannel = subChannel;
        IsOversea = false;
        IsNotCompatOnly = isNotCompatOnly;
    }
}