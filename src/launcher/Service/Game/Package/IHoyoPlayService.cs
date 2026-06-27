//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Service.Game.Scheme;
using Launcher.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Launcher.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Launcher.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;

namespace Launcher.Service.Game.Package;

internal interface IHoyoPlayService
{
    ValueTask<ValueResult<bool, GameBranchesWrapper>> TryGetBranchesAsync(LaunchScheme scheme);

    ValueTask<ValueResult<bool, GameChannelSDKsWrapper>> TryGetChannelSDKsAsync(LaunchScheme scheme);

    ValueTask<ValueResult<bool, DeprecatedFileConfigurationsWrapper>> TryGetDeprecatedFileConfigurationsAsync(LaunchScheme scheme);
}