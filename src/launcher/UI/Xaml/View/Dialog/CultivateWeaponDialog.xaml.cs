// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Metadata.Weapon;
using System.Collections.Immutable;

namespace Launcher.UI.Xaml.View.Dialog;

internal sealed partial class CultivateWeaponDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial CultivateWeaponDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, ImmutableArray<Weapon>>> SelectWeaponsAsync(ImmutableArray<Weapon> weapons)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        WeaponGridView.ItemsSource = weapons;

        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (result is ContentDialogResult.Primary && WeaponGridView.SelectedItems.Count > 0)
        {
            ImmutableArray<Weapon> selected = [.. WeaponGridView.SelectedItems.OfType<Weapon>()];
            return new(true, selected);
        }

        return new(false, default!);
    }
}
