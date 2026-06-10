//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core;

[Service(ServiceLifetime.Singleton)]
internal sealed class RuntimeOptions
{
    [Obsolete("This property only exist for binding purpose")]
    public Version Version { get => kyxsanRuntime.Version; }

    [Obsolete("This property only exist for binding purpose")]
    public string DataFolder { get => kyxsanRuntime.DataDirectory; }

    [Obsolete("This property only exist for binding purpose")]
    public string DeviceId { get => kyxsanRuntime.DeviceId; }

    [Obsolete("This property only exist for binding purpose")]
    public string WebView2Version { get => kyxsanRuntime.WebView2Version.Version; }

    [Obsolete("This property only exist for binding purpose")]
    public bool IsToastAvailable { get => kyxsanRuntime.IsAppNotificationEnabled; }
}