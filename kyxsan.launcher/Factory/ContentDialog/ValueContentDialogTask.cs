//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace kyxsan.Factory.ContentDialog;

internal readonly struct ValueContentDialogTask
{
    /// <summary>
    /// This task will be completed when the associated dialog finishes queueing and starts to show.
    /// </summary>
    public readonly Task QueueTask;

    /// <summary>
    /// This task will be completed when the associated dialog closed in any reason.
    /// </summary>
    public readonly Task<ContentDialogResult> ShowTask;

    public ValueContentDialogTask(Task queueTask, Task<ContentDialogResult> showTask)
    {
        QueueTask = queueTask;
        ShowTask = showTask;
    }
}