//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Windows.AppNotifications;

namespace kyxsan.Service.Notification;

[Service(ServiceLifetime.Singleton, typeof(IAppNotificationLifeTime))]
internal sealed partial class AppNotificationLifeTime : IAppNotificationLifeTime
{
    public void Dispose()
    {
        // 用于在程序退出时尝试清除所有的系统通知
        try
        {
            AppNotificationManager.Default.RemoveAllAsync().AsTask().GetAwaiter().GetResult();
            AppNotificationManager.Default.Unregister();
        }
        catch
        {
            // Ignored
        }
    }
}