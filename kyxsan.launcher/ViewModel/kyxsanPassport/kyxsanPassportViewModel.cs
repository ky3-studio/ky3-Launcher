//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Logging;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Navigation;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.UI.Xaml.View.Page;

namespace kyxsan.ViewModel.kyxsanPassport;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class kyxsanPassportViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial kyxsanPassportViewModel(IServiceProvider serviceProvider);

    public partial kyxsanUserOptions kyxsanUserOptions { get; }

    [Command("OpenTestPageCommand")]
    private async Task OpenTestPageAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Navigate to TestPage", "kyxsanPassportViewModel.Command"));
        await navigationService.NavigateAsync<TestPage>(NavigationExtraData.Default).ConfigureAwait(false);
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Register", "kyxsanPassportViewModel.Command"));

        kyxsanPassportRegisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<kyxsanPassportRegisterDialog>(serviceProvider).ConfigureAwait(false);

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
            messenger.Send(InfoBarMessage.Error(SH.ViewModelkyxsanPassportPasswordTooShortHint));
            return;
        }

        await kyxsanUserOptions.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Unregister", "kyxsanPassportViewModel.Command"));

        string? userName = await kyxsanUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        kyxsanPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<kyxsanPassportUnregisterDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        await kyxsanUserOptions.UnregisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Login", "kyxsanPassportViewModel.Command"));

        kyxsanPassportLoginDialog dialog = await contentDialogFactory.CreateInstanceAsync<kyxsanPassportLoginDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        await kyxsanUserOptions.LoginAsync(username, password, false).ConfigureAwait(false);
    }

    [Command("LogoutCommand")]
    private async Task LogoutAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Logout", "kyxsanPassportViewModel.Command"));
        await kyxsanUserOptions.LogoutAsync().ConfigureAwait(false);
    }

    [Command("ResetUsernameCommand")]
    private async Task ResetUsernameAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset email", "kyxsanPassportViewModel.Command"));
        string? userName = await kyxsanUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        kyxsanPassportResetUsernameDialog dialog = await contentDialogFactory.CreateInstanceAsync<kyxsanPassportResetUsernameDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? newUserName, string? verifyCode, string? newVerifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newUserName) || string.IsNullOrEmpty(verifyCode) || string.IsNullOrEmpty(newVerifyCode))
        {
            return;
        }

        await kyxsanUserOptions.ResetUserNameAsync(username, newUserName, verifyCode, newVerifyCode).ConfigureAwait(false);
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset password", "kyxsanPassportViewModel.Command"));
        string? userName = await kyxsanUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        kyxsanPassportResetPasswordDialog dialog = await contentDialogFactory.CreateInstanceAsync<kyxsanPassportResetPasswordDialog>(serviceProvider).ConfigureAwait(false);

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
            messenger.Send(InfoBarMessage.Error(SH.ViewModelkyxsanPassportPasswordTooShortHint));
            return;
        }

        await kyxsanUserOptions.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UseRedeemCodeCommand")]
    private async Task UseRedeemCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Use redeem code", "kyxsanPassportViewModel.Command"));
        kyxsanPassportUseRedeemCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<kyxsanPassportUseRedeemCodeDialog>(serviceProvider).ConfigureAwait(false);
        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, { Length: > 0 } redeemCode))
        {
            return;
        }

        await kyxsanUserOptions.UseRedeemCodeAsync(redeemCode).ConfigureAwait(false);
    }

    [Command("RefreshUserInfoCommand")]
    private async Task RefreshUserInfoAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh user info", "kyxsanPassportViewModel.Command"));
        await kyxsanUserOptions.RefreshUserInfoAsync().ConfigureAwait(false);
    }
}