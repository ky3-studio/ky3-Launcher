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

internal sealed class LaunchSchemeOversea : LaunchScheme
{
    private const string HoyoPlayLauncherOverseaId = "VYTpXlbWo8";
    private const string HoyoPlayGameOverseaId = "gopR6Cufr3";

    public LaunchSchemeOversea(ChannelType channel, SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = HoyoPlayLauncherOverseaId;
        GameId = HoyoPlayGameOverseaId;
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = true;
        IsNotCompatOnly = isNotCompatOnly;
    }
}