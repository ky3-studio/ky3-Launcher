//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Core.Logging;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Geetest;
using Launcher.Service.Notification;
using Launcher.Web.Hoyolab.Passport;
using Launcher.Web.Response;
using System.Runtime.CompilerServices;
using Windows.System;

namespace Launcher.UI.Xaml.View.Dialog;

internal sealed partial class UserAccountVerificationDialog : ContentDialog, IAigisProvider, INotifyPropertyChanged
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGeetestService geetestService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private const int CooldownSeconds = 60;

    private string? ticket;
    private bool isOversea;
    private DispatcherTimer? countdownTimer;
    private int remainingSeconds;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial UserAccountVerificationDialog(IServiceProvider serviceProvider);

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Email { get; set => SetProperty(ref field, value); }

    public string? Captcha
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                IsLoginEnabled = !string.IsNullOrEmpty(value) && value.Length is 6;
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }
    }

    public bool IsLoginEnabled { get; private set; }

    public bool IsSendEnabled { get; private set => SetProperty(ref field, value); } = true;

    public string? SendButtonText { get; private set => SetProperty(ref field, value); } = SH.ViewDialogUserMobileCaptchaSendCaptchaAction;

    public string? Aigis { get; set; }

    public async ValueTask<bool> TryValidateAsync(string ticket, bool isOversea)
    {
        this.ticket = ticket;
        this.isOversea = isOversea;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(isOversea);
            Response<ActionTicketInfo> resp = await passportClient.GetActionTicketInfoAsync(ticket).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(resp, serviceProvider, out ActionTicketInfo? actionTicketInfo))
            {
                return false;
            }

            taskContext.InvokeOnMainThread(() =>
            {
                Email = actionTicketInfo.UserInfo.Email;
                StartCountdown();
            });

            messenger.Send(InfoBarMessage.Information(SH.ViewDialogUserAccountVerificationEmailCaptchaSent));

            ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
            if (result is not ContentDialogResult.Primary)
            {
                return false;
            }

            ArgumentNullException.ThrowIfNull(Captcha);

            Response resp1 = await passportClient.VerifyActionTicketPartlyAsync(ticket, Captcha).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(resp1, serviceProvider))
            {
                return false;
            }

            Response<ActionTicketInfo> resp2 = await passportClient.GetActionTicketInfoAsync(ticket).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(resp2, serviceProvider, out actionTicketInfo))
            {
                return false;
            }

            return actionTicketInfo.VerifyInfo.Status is VerifyStatus.StatusVerified;
        }
    }

    private async void OnSendCaptchaClick(object sender, RoutedEventArgs e)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Send captcha", "UserAccountVerificationDialog.Command"));

        ArgumentNullException.ThrowIfNull(ticket);

        IsSendEnabled = false;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(isOversea);
            (string? rawSession, Response response) = await passportClient.CreateEmailCaptchaByActionTicketAsync(ticket, Aigis).ConfigureAwait(false);

            if (await geetestService.TryVerifyAigisSessionAsync(this, rawSession, isOversea).ConfigureAwait(false))
            {
                (_, response) = await passportClient.CreateEmailCaptchaByActionTicketAsync(ticket, Aigis).ConfigureAwait(false);
            }

            if (ResponseValidator.TryValidate(response, serviceProvider))
            {
                messenger.Send(InfoBarMessage.Information(SH.ViewDialogUserAccountVerificationEmailCaptchaSent));
                await taskContext.SwitchToMainThreadAsync();
                StartCountdown();
                return;
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        IsSendEnabled = true;
        SendButtonText = SH.ViewDialogUserMobileCaptchaSendCaptchaAction;
    }

    private void StartCountdown()
    {
        remainingSeconds = CooldownSeconds;
        IsSendEnabled = false;
        SendButtonText = $"{remainingSeconds}s";

        countdownTimer?.Stop();
        countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        countdownTimer.Tick += OnCountdownTick;
        countdownTimer.Start();
    }

    private void OnCountdownTick(object? sender, object e)
    {
        remainingSeconds--;

        if (remainingSeconds <= 0)
        {
            countdownTimer?.Stop();
            IsSendEnabled = true;
            SendButtonText = SH.ViewDialogUserMobileCaptchaSendCaptchaAction;
        }
        else
        {
            SendButtonText = $"{remainingSeconds}s";
        }
    }

    private void OnTextKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Enter)
        {
            e.Handled = true;
        }
    }

    private void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Enter)
        {
            e.Handled = true;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        return false;
    }
}