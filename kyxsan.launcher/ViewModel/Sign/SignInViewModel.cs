//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service;
using kyxsan.Service.Notification;
using kyxsan.Service.SignIn;
using kyxsan.Service.User;
using kyxsan.UI.Xaml.Data;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;
using kyxsan.Web.Response;
using kyxsan.Core.Setting;
using System.Collections.Immutable;
using kyxsan.Service.AutoSignIn;

namespace kyxsan.ViewModel.Sign;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class SignInViewModel : Abstraction.ViewModelSlim, IRecipient<UserAndUidChangedMessage>, IRecipient<SignInDataChangedMessage>
{
    private readonly WeakReference<ScrollViewer> weakScrollViewer = new(default!);

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly CultureOptions cultureOptions;
    private readonly ISignInService signInService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private bool updating;
    private int totalSignDay;
    private SignInRewardReSignInfo? resignInfo;

    private const string AutoSignInSettingKey = "SignIn.AutoSignInEnabled";

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial SignInViewModel(IServiceProvider serviceProvider);

    public UserAndUid? TargetUserAndUid { get; set; }

    [ObservableProperty]
    public partial IAdvancedCollectionView<AwardView>? Awards { get; set; }

    [ObservableProperty]
    public partial string? TotalSignInDaysHint { get; set; }

    [ObservableProperty]
    public partial string? CurrentUid { get; set; }

    [ObservableProperty]
    public partial bool IsTodaySigned { get; set; }

    [ObservableProperty]
    public partial bool IsAutoCheckIn { get; set; }

    private int pendingRefresh;
    private UserAndUid? pendingUserAndUid;
    private bool pendingPostSign;
    private bool pendingPostResign;

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is not { } userAndUid)
        {
            return;
        }

        if (TargetUserAndUid is { } target && userAndUid.User.InnerId != target.User.InnerId)
        {
            return;
        }

        if (TargetUserAndUid is not null)
        {
            TargetUserAndUid = userAndUid;
        }

        if (Volatile.Read(ref updating))
        {
            pendingUserAndUid = userAndUid;
            pendingPostSign = pendingPostSign || false;
            pendingPostResign = pendingPostResign || false;
            Interlocked.Exchange(ref pendingRefresh, 1);
            return;
        }

        UpdateSignInInfoAsync(userAndUid).SafeForget();
    }

    public void Receive(SignInDataChangedMessage message)
    {
        if (message.UserAndUid is not { } userAndUid)
        {
            return;
        }

        if (TargetUserAndUid is { } target && userAndUid.User.InnerId != target.User.InnerId)
        {
            return;
        }

        if (Volatile.Read(ref updating))
        {
            pendingUserAndUid = userAndUid;
            pendingPostSign = pendingPostSign || message.PostSign;
            pendingPostResign = pendingPostResign || false;
            Interlocked.Exchange(ref pendingRefresh, 1);
            return;
        }

        UpdateSignInInfoAsync(userAndUid, postSign: message.PostSign).SafeForget();
    }

    public void AttachXamlElement(ScrollViewer scrollViewer)
    {
        weakScrollViewer.SetTarget(scrollViewer);
    }

    protected override async Task LoadAsync()
    {
        try
        {
            IsAutoCheckIn = LocalSetting.Get(AutoSignInSettingKey, true);
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Warning(ex.Message));
        }

        if (TargetUserAndUid is { } userAndUid)
        {
            await UpdateSignInInfoAsync(userAndUid).ConfigureAwait(false);
        }
        else if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } currentUserAndUid)
        {
            await UpdateSignInInfoAsync(currentUserAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }
    }

    private static void InitializeClaimedAwards(ImmutableArray<AwardView> awards, int totalSignDay)
    {
        foreach (ref readonly AwardView awardView in awards.AsSpan(..totalSignDay))
        {
            awardView.IsClaimed = true;
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateSignInInfoAsync(UserAndUid userAndUid, bool postSign = false, bool postResign = false)
    {
        if (Interlocked.Exchange(ref updating, true))
        {
            return;
        }

        try
        {
            await taskContext.SwitchToBackgroundAsync();

            Reward? reward;
            SignInRewardInfo? info;
            SignInRewardReSignInfo? resignInfo;
            using (IServiceScope scope = ServiceProvider.CreateScope())
            {
                ISignInClient signInClient = scope.ServiceProvider
                    .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                    .Create(userAndUid.IsOversea);

                Task<Response<Reward>> rewardTask = signInClient.GetRewardAsync(userAndUid.User).AsTask();
                Task<Response<SignInRewardInfo>> infoTask = signInClient.GetInfoAsync(userAndUid).AsTask();
                Task<Response<SignInRewardReSignInfo>> resignInfoTask = signInClient.GetResignInfoAsync(userAndUid).AsTask();

                await Task.WhenAll(rewardTask, infoTask, resignInfoTask).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(rewardTask.Result, scope.ServiceProvider, out reward))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceSignInRewardListRequestFailed));
                    return;
                }

                if (!ResponseValidator.TryValidate(infoTask.Result, scope.ServiceProvider, out info))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceSignInInfoRequestFailed));
                    return;
                }

                if (!ResponseValidator.TryValidate(resignInfoTask.Result, scope.ServiceProvider, out resignInfo))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceSignInInfoRequestFailed));
                    return;
                }
            }

            if (postSign || postResign)
            {
                Award award = reward.Awards[Math.Clamp(info.TotalSignDay - 1, 0, reward.Awards.Length - 1)];

                if (postSign)
                {
                    messenger.Send(InfoBarMessage.Success(SH.FormatServiceSignInSuccessReward(award.Name, award.Count)));
                }
                else if (postResign)
                {
                    messenger.Send(InfoBarMessage.Success(SH.FormatServiceReSignInSuccessReward(award.Name, award.Count)));
                }
            }

            ImmutableArray<AwardView> views = reward.Awards.SelectAsArray(AwardView.Create);
            InitializeClaimedAwards(views, info.TotalSignDay);

            totalSignDay = info.TotalSignDay;
            this.resignInfo = resignInfo;

            IAdvancedCollectionView<AwardView> advancedViews = views.AsAdvancedCollectionView();
            await taskContext.SwitchToMainThreadAsync();
            IsTodaySigned = info.IsSign;
            Awards = advancedViews;
            CurrentUid = userAndUid.Uid.ToString();

            string monthName = cultureOptions.CurrentCulture.Value.DateTimeFormat.MonthNames[reward.Month - 1];
            TotalSignInDaysHint = SH.FormatViewModelSignInTotalSignInDaysHint(monthName, info.TotalSignDay);
            ScrollToCurrentOrNextAward(postSign || postResign);

            IsInitialized = true;
        }
        catch (ObjectDisposedException)
        {
        }
        finally
        {
            Volatile.Write(ref updating, false);

            if (Interlocked.Exchange(ref pendingRefresh, 0) == 1 && pendingUserAndUid is { } pending)
            {
                bool queuedPostSign = pendingPostSign;
                bool queuedPostResign = pendingPostResign;

                pendingUserAndUid = null;
                pendingPostSign = false;
                pendingPostResign = false;

                UpdateSignInInfoAsync(pending, postSign: queuedPostSign, postResign: queuedPostResign).SafeForget();
            }
        }
    }

    [Command("ScrollToNextAwardCommand")]
    private void ScrollToNextAward()
    {
        ScrollToCurrentOrNextAward();
    }

    private void ScrollToCurrentOrNextAward(bool current = false)
    {
        if (!weakScrollViewer.TryGetTarget(out ScrollViewer? scrollViewer))
        {
            return;
        }

        DateTime now = DateTime.Now;
        int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
        int rows = (int)Math.Ceiling(daysInMonth / 7.0);

        int rowIndex = (Math.Clamp(current ? totalSignDay : totalSignDay + 1, 1, daysInMonth) - 1) / 7;
        double offset = rowIndex * (scrollViewer.ExtentHeight / rows);
        scrollViewer.ChangeView(null, offset, null);
    }

    [Command("ClaimSignInRewardCommand")]
    private async Task ClaimSignInRewardAsync()
    {
        UserAndUid? userAndUid = TargetUserAndUid
            ?? await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);

        if (userAndUid is null)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        if (await signInService.ClaimSignInRewardAsync(userAndUid).ConfigureAwait(false))
        {
            await UpdateSignInInfoAsync(userAndUid, postSign: true).ConfigureAwait(false);
        }
    }

    [Command("ClaimResignInRewardCommand")]
    private async Task ClaimResignInRewardAsync()
    {
        UserAndUid? userAndUid = TargetUserAndUid
            ?? await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);

        if (userAndUid is null)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        string content = userAndUid.IsOversea
            ? SH.FormatViewModelSignInReSignInDialogContentOversea(resignInfo?.Cost, resignInfo?.QualityCount)
            : SH.FormatViewModelSignInReSignInDialogContent(resignInfo?.CoinCost, resignInfo?.CoinCount);

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(SH.ViewModelSignInReSignInDialogTitle, content).ConfigureAwait(false);
        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        if (await signInService.ClaimResignInRewardAsync(userAndUid).ConfigureAwait(false))
        {
            await UpdateSignInInfoAsync(userAndUid, postResign: true).ConfigureAwait(false);
        }
    }

    partial void OnIsAutoCheckInChanged(bool value)
    {
        try
        {
            LocalSetting.Set(AutoSignInSettingKey, value);
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Warning(ex.Message));
        }
    }
}