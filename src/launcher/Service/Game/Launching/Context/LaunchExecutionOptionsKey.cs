//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Diagnostics;

namespace Launcher.Service.Game.Launching.Context;

internal readonly struct LaunchExecutionOptionsKey<TValue>
{
    public LaunchExecutionOptionsKey(string key)
    {
        Key = key;
    }

    public string Key { get; }
}

internal sealed class LaunchExecutionOptionsKey
{
    public static readonly LaunchExecutionOptionsKey<bool> ChannelOptionsChanged = new(nameof(ChannelOptionsChanged));
    public static readonly LaunchExecutionOptionsKey<string> LoginAuthTicket = new(nameof(LoginAuthTicket));
    public static readonly LaunchExecutionOptionsKey<IProcess> RunningProcess = new(nameof(RunningProcess));
}
