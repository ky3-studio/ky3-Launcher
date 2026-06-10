//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using kyxsan.Core.DataTransfer;
using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Core.Logging;
using kyxsan.Core.Setting;
using kyxsan.Service;
using kyxsan.Service.Announcement;
using kyxsan.Service.Notification;
using kyxsan.Service.User;
using kyxsan.Web.Hoyolab.Bbs.Home;
using kyxsan.Web.Hoyolab.Hk4e.Common.Announcement;
using kyxsan.Web.Hoyolab.Takumi.Event.Miyolive;
using kyxsan.Web.Response;
using kyxsan.Web.WebView2;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace kyxsan.ViewModel.Home;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;
    private readonly IServiceProvider serviceProvider;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private DispatcherTimer? timeRefreshTimer;

    [GeneratedConstructor]
    public partial AnnouncementViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial AnnouncementWrapper? Announcement { get; set; }

    [ObservableProperty]
    public partial string GreetingText { get; set; } = SH.ViewPageHomeGreetingTextDefault;

    [ObservableProperty]
    public partial ImmutableArray<CodeWrapper> RedeemCodes { get; set; } = [];

    [GeneratedRegex("act_id=(.*?)&")]
    private static partial Regex ActIdExtractor { get; }

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        _ = CoreWebView2EnvironmentFactory.GetAsync();
        InitializeInGameAnnouncementAsync(token).SafeForget();
        InitializeMiyoliveCodeAsync(token).SafeForget();
        UpdateGreetingText();
        return ValueTask.FromResult(true);
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeInGameAnnouncementAsync(CancellationToken token)
    {
        try
        {
            AnnouncementWrapper? announcementWrapper = await announcementService.GetAnnouncementWrapperAsync(cultureOptions.LanguageCode, appOptions.Region.Value, token).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = announcementWrapper;
            DeferContentLoader?.Load("GameAnnouncementPivot");
            StartTimeRefreshTimer();
        }
        catch (OperationCanceledException)
        {
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeMiyoliveCodeAsync(CancellationToken token)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { IsOversea: false } userAndUid)
            {
                // The oversea user can direct use their code on the official website.
                return;
            }

            IHomeClient homeClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IHomeClient>>()
                .CreateFor(userAndUid);

            Response<NewHomeNewInfo> newHomeInfoResponse = await homeClient.GetNewHomeInfoAsync(2, token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidateWithoutUINotification(newHomeInfoResponse, out NewHomeNewInfo? newHomeInfo))
            {
                return;
            }

            Uri url;
            if (newHomeInfo.Lives is [{ Data.LiveUrl: { } url1 }, ..])
            {
                url = url1;
            }
            else if (newHomeInfo.Navigator.SingleOrDefault(nav => nav.Name.EqualsAny(["直播兑换码", "前瞻直播"], StringComparison.OrdinalIgnoreCase)) is { AppPath: { } url2 })
            {
                url = url2;
            }
            else
            {
                return;
            }

            if (ActIdExtractor.Match(url.OriginalString) is not { Success: true, Groups: [_, { Value: { } actId }, ..] })
            {
                return;
            }

            IMiyoliveClient miyoliveClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IMiyoliveClient>>()
                .CreateFor(userAndUid);

            Response<CodeListWrapper> codeListResponse = await miyoliveClient.RefreshCodeAsync(actId, token).ConfigureAwait(false);
            if (!ResponseValidator.TryValidateWithoutUINotification(codeListResponse, out CodeListWrapper? wrapper))
            {
                return;
            }

            ImmutableArray<CodeWrapper> wrappers = wrapper.CodeList.SelectAsArray(static wrapper => wrapper.WithTitle(wrapper.Title.DecodeHtml()));
            wrappers = [.. wrappers.Where(static wrapper => !string.IsNullOrEmpty(wrapper.Code))];
            if (wrappers.IsEmpty)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            RedeemCodes = wrappers;
        }
    }

    [Command("CopyCodeCommand")]
    private async Task CopyCodeToClipboardAsync(string? code)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy redeem code to ClipBoard", "AnnouncementPage.Command"));

        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        await serviceProvider.GetRequiredService<IClipboardProvider>().SetTextAsync(code).ConfigureAwait(false);
        serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Success(SH.ViewPageAnnouncementRedeemCodeCopySucceed));
    }

    private void UpdateGreetingText()
    {
        int rand = Random.Shared.Next(0, 1000);

        if (rand is >= 0 and < 6)
        {
            GreetingText = SH.ViewPageHomeGreetingTextEasterEgg;
        }
        else if (rand is >= 6 and < 57)
        {
            GreetingText = SH.FormatViewPageHomeGreetingTextCommon2(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
        }
        else if (rand is >= 57 and < 1000)
        {
            GreetingText = SH.FormatViewPageHomeGreetingTextCommon2(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
        }
    }

    private void StartTimeRefreshTimer()
    {
        if (timeRefreshTimer is not null)
        {
            return;
        }

        timeRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timeRefreshTimer.Tick += OnTimeRefreshTimerTick;
        timeRefreshTimer.Start();
    }

    private void OnTimeRefreshTimerTick(object? sender, object e)
    {
        if (Announcement?.List is not { } wrappers)
        {
            return;
        }

        foreach (AnnouncementListWrapper wrapper in wrappers)
        {
            foreach (Announcement item in wrapper.List)
            {
                if (item.StartTime != default && item.EndTime != default)
                {
                    item.NotifyTimePropertiesChanged();
                }
            }
        }
    }

    protected override void UninitializeOverride()
    {
        if (timeRefreshTimer is not null)
        {
            timeRefreshTimer.Stop();
            timeRefreshTimer.Tick -= OnTimeRefreshTimerTick;
            timeRefreshTimer = null;
        }
    }
}