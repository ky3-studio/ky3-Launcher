//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Microsoft.UI.Xaml.Controls;

namespace kyxsan.Factory.ContentDialog;

internal interface IContentDialogFactory
{
    bool IsDialogShowing { get; }

    ITaskContext TaskContext { get; }

    ValueTask<ContentDialogResult> CreateForConfirmAsync([LocalizationRequired] string title, [LocalizationRequired] string content);

    ValueTask<ContentDialogResult> CreateForConfirmCancelAsync([LocalizationRequired] string title, [LocalizationRequired] string content, ContentDialogButton defaultButton = ContentDialogButton.Close, bool isPrimaryButtonEnabled = true);

    ValueTask<ContentDialogResult> CreateForSelectionAsync([LocalizationRequired] string title, [LocalizationRequired] string content, [LocalizationRequired] string primaryButtonText, [LocalizationRequired] string secondaryButtonText);

    ValueTask<Microsoft.UI.Xaml.Controls.ContentDialog> CreateForIndeterminateProgressAsync([LocalizationRequired] string title);

    ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(IServiceProvider serviceProvider, params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog;

    ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog);
}