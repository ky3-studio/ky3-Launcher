//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Factory.ContentDialog;
using WinRT;

namespace kyxsan.UI.Xaml.View.Dialog;

[DependencyProperty<string>("Text", PropertyChangedCallbackName = nameof(OnTextChanged))]
internal sealed partial class ReconfirmDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial ReconfirmDialog(IServiceProvider serviceProvider);

    public string ConfirmText { get; private set; } = default!;

    public async ValueTask<bool> ConfirmAsync(string confirmText)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        ConfirmText = confirmText;
        return await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary;
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ReconfirmDialog dialog = sender.As<ReconfirmDialog>();
        dialog.IsPrimaryButtonEnabled = string.Equals(dialog.Text, dialog.ConfirmText, StringComparison.Ordinal);
    }
}