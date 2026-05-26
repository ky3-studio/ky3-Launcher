//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Package;
using kyxsan.Service.Game.Scheme;
using kyxsan.ViewModel.Game;
using System.Collections.Concurrent;

namespace kyxsan.Service.Game.Launching.Context;

internal sealed class BeforeLaunchExecutionContext
{
    private readonly ConcurrentDictionary<string, object?> options = [];

    public required IViewModelSupportLaunchExecution ViewModel { get; init; }

    public required IProgress<LaunchStatus?> Progress { get; init; }

    public required IServiceProvider ServiceProvider { get; init; }

    public required ITaskContext TaskContext { get; init; }

    public required IGameFileSystem FileSystem { get; set; }

    public required IHoyoPlayService HoyoPlay { get; init; }

    public required IMessenger Messenger { get; init; }

    public required LaunchOptions LaunchOptions { get; init; }

    public required LaunchScheme CurrentScheme { get; init; }

    public required LaunchScheme TargetScheme { get; init; }

    public required GameIdentity Identity { get; init; }

    public bool TryGetOption<TValue>(LaunchExecutionOptionsKey<TValue> key, [MaybeNullWhen(false)] out TValue value)
    {
        if (options.TryGetValue(key.Key, out object? objValue) && objValue is TValue tValue)
        {
            value = tValue;
            return true;
        }

        value = default;
        return false;
    }

    public void SetOption<TValue>(LaunchExecutionOptionsKey<TValue> key, TValue value)
    {
        options[key.Key] = value;
    }
}