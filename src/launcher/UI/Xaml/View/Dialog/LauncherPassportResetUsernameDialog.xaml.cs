//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Launcher.Core.Logging;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Notification;
using Launcher.Web.Launcher;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Response;

namespace Launcher.UI.Xaml.View.Dialog;

[DependencyProperty<string>("UserName")]
[DependencyProperty<string>("NewUserName")]
[DependencyProperty<string>("VerifyCode")]
[DependencyProperty<string>("NewVerifyCode")]
internal sealed partial class LauncherPassportResetUsernameDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMessenger messenger;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial LauncherPassportResetUsernameDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, (string? UserName, string? NewUserName, string? VerifyCode, string? NewVerifyCode)>> GetInputAsync(string? userName)
    {
        InitializeUserNameTextBox(userName);
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, NewUserName, VerifyCode, NewVerifyCode));
    }

    [Command("VerifyOldCommand")]
    private async Task VerifyOldAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify old username", "LauncherPassportResetUsernameDialog.Command"));
        await PrivateVerifyAsync(UserName, VerifyCodeRequestType.ResetUserName).ConfigureAwait(false);
    }

    [Command("VerifyNewCommand")]
    private async Task VerifyNewAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify new username", "LauncherPassportResetUsernameDialog.Command"));
        await PrivateVerifyAsync(NewUserName, VerifyCodeRequestType.ResetUserNameNew).ConfigureAwait(false);
    }

    private async ValueTask PrivateVerifyAsync(string? userName, VerifyCodeRequestType type)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }

        if (!userName.IsEmail())
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelLauncherPassportEmailNotValidHint));
            return;
        }

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            LauncherPassportClient LauncherPassportClient = scope.ServiceProvider.GetRequiredService<LauncherPassportClient>();

            LauncherResponse response = await LauncherPassportClient.RequestVerifyAsync(userName, type).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                return;
            }

            messenger.Send(InfoBarMessage.Information(response.GetLocalizationMessage()));
        }
    }

    private void InitializeUserNameTextBox(string? userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }

        UserName = userName;
        UserNameTextBox.IsEnabled = false;
    }
}
