//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Logging;
using Launcher.Service.Game;
using Launcher.Service.Game.Scheme;
using Launcher.Service.Notification;
using Launcher.Service.User;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.ViewModel.User;
using Launcher.Web.Hoyolab.Takumi.Binding;
using System.Collections.Immutable;
using System.IO;

namespace Launcher.ViewModel.Game;

internal sealed partial class LaunchGameViewModel
{
    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch game", "LaunchGameViewModel.Command"));

        if (LaunchOptions.AdvancedStartDelayedOnGameLaunch.Value)
        {
            Shared.LaunchAdvancedDelayedAsync().SafeForget();
        }

        UserAndUid? userAndUid;
        if (LaunchOptions.UsingHoyolabAccount.Value && SelectedLaunchUser is not null)
        {
            if (SelectedLaunchUser.UserGameRoles.CurrentItem is null)
            {
                bool restored = false;
                string? preferredUid = SelectedLaunchUser.PreferredUid;
                if (!string.IsNullOrEmpty(preferredUid))
                {
                    foreach (UserGameRole role in SelectedLaunchUser.UserGameRoles)
                    {
                        if (role.GameUid == preferredUid)
                        {
                            SelectedLaunchUser.UserGameRoles.MoveCurrentTo(role);
                            restored = true;
                            break;
                        }
                    }
                }

                if (!restored)
                {
                    SelectedLaunchUser.UserGameRoles.MoveCurrentToFirst();
                }
            }

            UserAndUid.TryFromUser(SelectedLaunchUser, out userAndUid);
        }
        else
        {
            userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        }

        await Shared.DefaultLaunchExecutionAsync(this, userAndUid).ConfigureAwait(false);
    }

    [Command("ConvertCommand")]
    private async Task ConvertAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Convert game server", "LaunchGameViewModel.Command"));
        LaunchScheme? previousScheme = Shared.GetCurrentLaunchSchemeFromConfigurationFile(false);
        bool success = await Shared.ConvertLaunchExecutionAsync(this).ConfigureAwait(false);
        await HandleGamePathEntryChangeAsync().ConfigureAwait(false);

        if (success)
        {
            LaunchScheme? newScheme = Shared.GetCurrentLaunchSchemeFromConfigurationFile(false);
            if (newScheme is not null && !ReferenceEquals(newScheme, previousScheme))
            {
                string serverName = newScheme.DisplayName;
                string desc = newScheme.Description;
                string message = string.IsNullOrEmpty(desc)
                    ? string.Format(SH.ViewModelServerConvertSuccessText, serverName)
                    : string.Format(SH.ViewModelServerConvertSuccessWithDescText, serverName, desc);
                messenger.Send(InfoBarMessage.Success(message));
            }
        }
    }

    [Command("CleanConvertResourceCommand")]
    private async Task CleanConvertResourceAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Clean convert resource", "LaunchGameViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            LaunchGameCleanResourceDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<LaunchGameCleanResourceDialog>(scope.ServiceProvider)
                .ConfigureAwait(false);

            ImmutableArray<CleanResourceItem> selectedItems = await dialog.GetSelectedItemsAsync().ConfigureAwait(false);

            if (selectedItems.IsDefaultOrEmpty)
            {
                return;
            }

            long totalCleaned = 0;
            int successCount = 0;
            List<string> errors = [];

            foreach (CleanResourceItem item in selectedItems)
            {
                try
                {
                    if (Directory.Exists(item.Path))
                    {
                        Directory.Delete(item.Path, true);
                        totalCleaned += item.Size;
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{item.Name}: {ex.Message}");
                }
            }

            if (successCount > 0)
            {
                string sizeText = LaunchGameCleanResourceDialog.FormatSize(totalCleaned);
                messenger.Send(InfoBarMessage.Success(string.Format(SH.ViewModelCleanResourceSuccessText, successCount, sizeText)));
            }

            if (errors.Count > 0)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelCleanResourcePartialError, string.Join("\n", errors)));
            }
        }
    }

    [Command("KillGameProcessCommand")]
    private async Task KillGameProcess()
    {
        await GameLifeCycle.TryKillGameProcessAsync(taskContext).ConfigureAwait(false);
    }

    [Command("RemoveAspectRatioCommand")]
    private void RemoveAspectRatio(AspectRatio? aspectRatio)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove aspect ratio", "LaunchGameViewModel.Command"));
        if (aspectRatio is null)
        {
            return;
        }

        if (aspectRatio.Equals(LaunchOptions.SelectedAspectRatio))
        {
            LaunchOptions.SelectedAspectRatio = default;
        }

        LaunchOptions.AspectRatios.Remove(aspectRatio);
    }
}
