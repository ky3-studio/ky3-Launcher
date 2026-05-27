//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core.Logging;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.Notification;
using kyxsan.Web.kyxsan;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Response;

namespace kyxsan.UI.Xaml.View.Dialog;

[DependencyProperty<string>("UserName")]
[DependencyProperty<string>("NewUserName")]
[DependencyProperty<string>("VerifyCode")]
[DependencyProperty<string>("NewVerifyCode")]
internal sealed partial class kyxsanPassportResetUsernameDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMessenger messenger;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial kyxsanPassportResetUsernameDialog(IServiceProvider serviceProvider);

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
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify old username", "kyxsanPassportResetUsernameDialog.Command"));
        await PrivateVerifyAsync(UserName, VerifyCodeRequestType.ResetUserName).ConfigureAwait(false);
    }

    [Command("VerifyNewCommand")]
    private async Task VerifyNewAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify new username", "kyxsanPassportResetUsernameDialog.Command"));
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
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelkyxsanPassportEmailNotValidHint));
            return;
        }

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            kyxsanPassportClient kyxsanPassportClient = scope.ServiceProvider.GetRequiredService<kyxsanPassportClient>();

            kyxsanResponse response = await kyxsanPassportClient.RequestVerifyAsync(userName, type).ConfigureAwait(false);
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