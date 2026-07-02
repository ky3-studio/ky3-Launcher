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
using Launcher.Service.Launcher;
using Launcher.Service.Navigation;
using Launcher.Service.Notification;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.UI.Xaml.View.Page;

namespace Launcher.ViewModel.LauncherPassport;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class LauncherPassportViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial LauncherPassportViewModel(IServiceProvider serviceProvider);

    public partial LauncherUserOptions LauncherUserOptions { get; }

    [Command("OpenTestPageCommand")]
    private async Task OpenTestPageAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Navigate to TestPage", "LauncherPassportViewModel.Command"));
        await navigationService.NavigateAsync<TestPage>(NavigationExtraData.Default).ConfigureAwait(false);
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Register", "LauncherPassportViewModel.Command"));

        LauncherPassportRegisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<LauncherPassportRegisterDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        if (password.Length < 8)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelLauncherPassportPasswordTooShortHint));
            return;
        }

        await LauncherUserOptions.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Unregister", "LauncherPassportViewModel.Command"));

        string? userName = await LauncherUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        LauncherPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<LauncherPassportUnregisterDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        await LauncherUserOptions.UnregisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Login", "LauncherPassportViewModel.Command"));

        LauncherPassportLoginDialog dialog = await contentDialogFactory.CreateInstanceAsync<LauncherPassportLoginDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        await LauncherUserOptions.LoginAsync(username, password, false).ConfigureAwait(false);
    }

    [Command("LogoutCommand")]
    private async Task LogoutAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Logout", "LauncherPassportViewModel.Command"));
        await LauncherUserOptions.LogoutAsync().ConfigureAwait(false);
    }

    [Command("ResetUsernameCommand")]
    private async Task ResetUsernameAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset email", "LauncherPassportViewModel.Command"));
        string? userName = await LauncherUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        LauncherPassportResetUsernameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LauncherPassportResetUsernameDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? newUserName, string? verifyCode, string? newVerifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newUserName) || string.IsNullOrEmpty(verifyCode) || string.IsNullOrEmpty(newVerifyCode))
        {
            return;
        }

        await LauncherUserOptions.ResetUserNameAsync(username, newUserName, verifyCode, newVerifyCode).ConfigureAwait(false);
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset password", "LauncherPassportViewModel.Command"));
        string? userName = await LauncherUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        LauncherPassportResetPasswordDialog dialog = await contentDialogFactory.CreateInstanceAsync<LauncherPassportResetPasswordDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        if (password.Length < 8)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelLauncherPassportPasswordTooShortHint));
            return;
        }

        await LauncherUserOptions.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UseRedeemCodeCommand")]
    private async Task UseRedeemCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Use redeem code", "LauncherPassportViewModel.Command"));
        LauncherPassportUseRedeemCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<LauncherPassportUseRedeemCodeDialog>(serviceProvider).ConfigureAwait(false);
        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, { Length: > 0 } redeemCode))
        {
            return;
        }

        await LauncherUserOptions.UseRedeemCodeAsync(redeemCode).ConfigureAwait(false);
    }

    [Command("RefreshUserInfoCommand")]
    private async Task RefreshUserInfoAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh user info", "LauncherPassportViewModel.Command"));
        await LauncherUserOptions.RefreshUserInfoAsync().ConfigureAwait(false);
    }
}
