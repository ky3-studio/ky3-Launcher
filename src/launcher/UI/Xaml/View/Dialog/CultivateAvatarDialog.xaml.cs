// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using kyxsan.Factory.ContentDialog;
using kyxsan.Model.Metadata.Avatar;
using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.View.Dialog;

internal sealed partial class CultivateAvatarDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial CultivateAvatarDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, ImmutableArray<Avatar>>> SelectAvatarsAsync(ImmutableArray<Avatar> avatars)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        AvatarGridView.ItemsSource = avatars;

        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (result is ContentDialogResult.Primary && AvatarGridView.SelectedItems.Count > 0)
        {
            ImmutableArray<Avatar> selected = [.. AvatarGridView.SelectedItems.OfType<Avatar>()];
            return new(true, selected);
        }

        return new(false, default!);
    }
}
