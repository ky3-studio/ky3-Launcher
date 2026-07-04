//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Logging;
using Launcher.Factory.ContentDialog;
using Launcher.Model.Entity;
using Launcher.Service.Game;
using Launcher.Service.Notification;
using Launcher.UI.Xaml.View.Dialog;

namespace Launcher.ViewModel.Game;

internal sealed partial class LaunchGameViewModel
{
    [Command("DetectGameAccountCommand")]
    private async Task DetectGameAccountAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Detect registry game account", "LaunchGameViewModel.Command"));

        try
        {
            if (TargetSchemeFilteredGameAccountsView.Scheme is null)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameSchemeNotSelected));
                return;
            }

            if (TargetSchemeFilteredGameAccountsView.View is null)
            {
                return;
            }

            GameAccount? currentAccount = await gameService.DetectGameAccountAsync(TargetSchemeFilteredGameAccountsView.Scheme, async (suggestedName) =>
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                        .GetRequiredService<IContentDialogFactory>()
                        .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider, suggestedName)
                        .ConfigureAwait(false);
                    return await dialog.GetInputNameAsync().ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            if (currentAccount is not null)
            {
                await taskContext.SwitchToMainThreadAsync();
                TargetSchemeFilteredGameAccountsView.View.MoveCurrentTo(currentAccount);
            }
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("ModifyGameAccountCommand")]
    private async Task ModifyGameAccountAsync(GameAccount? gameAccount)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Modify registry game account", "LaunchGameViewModel.Command"));

        if (gameAccount is null)
        {
            return;
        }

        await gameService.ModifyGameAccountAsync(gameAccount, async originalName =>
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                LaunchGameAccountNameDialog dialog = await scope.ServiceProvider
                    .GetRequiredService<IContentDialogFactory>()
                    .CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider, originalName)
                    .ConfigureAwait(false);

                return await dialog.GetInputNameAsync().ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    [Command("RemoveGameAccountCommand")]
    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove registry game account", "LaunchGameViewModel.Command"));

        if (gameAccount is null)
        {
            return;
        }

        await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
    }
}
