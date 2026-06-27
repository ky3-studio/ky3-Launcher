// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Entity;

namespace Launcher.UI.Xaml.View.Dialog;

[DependencyProperty<string>("Text")]
internal sealed partial class CultivateProjectDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial CultivateProjectDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, CultivateProject>> CreateProjectAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (result is ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(Text))
        {
            CultivateProject project = CultivateProject.From(Text.Trim(), TimeSpan.FromHours(8));
            return new(true, project);
        }

        return new(false, default!);
    }
}
