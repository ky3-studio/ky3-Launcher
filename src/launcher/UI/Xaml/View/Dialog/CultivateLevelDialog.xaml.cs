// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Weapon;
using Launcher.ViewModel.AvatarProperty;
using System.Collections.Immutable;

namespace Launcher.UI.Xaml.View.Dialog;

internal sealed partial class CultivateLevelDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial CultivateLevelDialog(IServiceProvider serviceProvider);

    private List<CultivateAvatarLevelItem> Items { get; set; } = [];

    public async ValueTask<ValueResult<bool, ImmutableArray<CultivateAvatarLevelItem>>> SelectLevelsAsync(
        ImmutableArray<(Avatar Avatar, CharacterView? CharacterView, Weapon? Weapon)> entries)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        Items = [];
        foreach ((Avatar avatar, CharacterView? characterView, Weapon? weapon) in entries)
        {
            Items.Add(CultivateAvatarLevelItem.Create(avatar, characterView, weapon));
        }

        AvatarItemsControl.ItemsSource = Items;

        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (result is ContentDialogResult.Primary && Items.Count > 0)
        {
            return new(true, [.. Items]);
        }

        return new(false, default!);
    }
}

internal sealed class CultivateLevelInput
{
    public uint AvatarLevelFrom { get; init; }
    public uint AvatarLevelTo { get; init; }
    public uint SkillALevelFrom { get; init; }
    public uint SkillALevelTo { get; init; }
    public uint SkillELevelFrom { get; init; }
    public uint SkillELevelTo { get; init; }
    public uint SkillQLevelFrom { get; init; }
    public uint SkillQLevelTo { get; init; }
    public uint WeaponLevelFrom { get; init; }
    public uint WeaponLevelTo { get; init; }
    public bool WeaponAscended { get; init; }
}
