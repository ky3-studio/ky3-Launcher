//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan;

internal static class XamlApplicationLifetime
{
    public static bool DispatcherQueueInitialized { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool CultureInfoInitialized { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool NotifyIconCreated { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool ActivationAndInitializationCompleted { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool IsFirstRunAfterUpdate { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool Exiting { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }

    public static bool Exited { get => Volatile.Read(ref field); set => Volatile.Write(ref field, value); }
}