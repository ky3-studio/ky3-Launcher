// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using kyxsan.Factory.ContentDialog;
using kyxsan.Model.Metadata.Weapon;
using System.Collections.Immutable;

namespace kyxsan.UI.Xaml.View.Dialog;

internal sealed partial class CultivateWeaponLevelDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial CultivateWeaponLevelDialog(IServiceProvider serviceProvider);

    private List<CultivateWeaponLevelItem> Items { get; set; } = [];

    public async ValueTask<ValueResult<bool, ImmutableArray<CultivateWeaponLevelItem>>> SelectLevelsAsync(ImmutableArray<Weapon> weapons)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        Items = [];
        foreach (Weapon weapon in weapons)
        {
            Items.Add(CultivateWeaponLevelItem.Create(weapon));
        }

        WeaponItemsControl.ItemsSource = Items;

        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (result is ContentDialogResult.Primary && Items.Count > 0)
        {
            return new(true, [.. Items]);
        }

        return new(false, default!);
    }
}
