//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Core.Setting;
using Launcher.Service.Notification;
using Launcher.Service.SignIn;
using Launcher.Service.User;
using Launcher.ViewModel.User;
using Launcher.Web.Hoyolab;
using Launcher.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Launcher.Web.Response;
using BindingUser = Launcher.ViewModel.User.User;
using EntityUser = Launcher.Model.Entity.User;

namespace Launcher.Service.AutoSignIn;

[Service(ServiceLifetime.Singleton, typeof(IAutoSignInService))]
internal sealed partial class AutoSignInService : IAutoSignInService
{
    private const string AutoSignInEnabledKeyPrefix = "SignIn.AutoSignIn.Enabled.";
    private const string AutoSignInLastAttemptDayKeyPrefix = "SignIn.AutoSignIn.LastAttemptDayKey.";
    private const string AutoSignInLastFailureUtcTicksPrefix = "SignIn.AutoSignIn.LastFailureUtcTicks.";

    private static readonly TimeSpan FailureCooldown = TimeSpan.FromMinutes(10);

    private readonly IServiceProvider serviceProvider;
    private readonly ISignInService signInService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial AutoSignInService(IServiceProvider serviceProvider);

    public async ValueTask<bool> RunAsync(CancellationToken token = default)
    {
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
        bool anySuccess = false;

        foreach (BindingUser user in users.Source)
        {
            if (user.UserGameRoles.CurrentItem is null)
            {
                await taskContext.SwitchToMainThreadAsync();
                user.UserGameRoles.MoveCurrentToFirst();
                await taskContext.SwitchToBackgroundAsync();
            }

            if (!UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
            {
                continue;
            }

            if (await OnUserAndUidChangedAsync(userAndUid, token).ConfigureAwait(false))
            {
                anySuccess = true;
            }
        }

        return anySuccess;
    }

    public async ValueTask<bool> OnUserAndUidChangedAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        string uidString = userAndUid.Uid.ToString();

        if (!LocalSetting.Get(AutoSignInEnabledKeyPrefix + uidString, true))
        {
            return false;
        }
        string completedDayKeySetting = AutoSignInLastAttemptDayKeyPrefix + uidString;
        string lastFailureTicksKey = AutoSignInLastFailureUtcTicksPrefix + uidString;
        string serverDayKey = GetServerDayKey(userAndUid.Uid);

        // Dedupe by server day when we already know the user is signed or we have signed successfully.
        if (LocalSetting.Get(completedDayKeySetting, string.Empty) == serverDayKey)
        {
            return false;
        }

        // Throttle repeated failures to reduce spam while still allowing retries later.
        long lastFailureTicks = LocalSetting.Get(lastFailureTicksKey, 0L);
        if (lastFailureTicks != 0)
        {
            DateTimeOffset lastFailure = new(lastFailureTicks, TimeSpan.Zero);
            if (DateTimeOffset.UtcNow - lastFailure < FailureCooldown)
            {
                return false;
            }
        }

        try
        {
            await taskContext.SwitchToBackgroundAsync();

            // 1) Check current server sign-in state first.
            Response<SignInRewardInfo> infoResponse = await GetSignInInfoAsync(userAndUid, token).ConfigureAwait(false);
            if (infoResponse is { ReturnCode: 0, Data: { } data } && data.IsSign)
            {
                LocalSetting.Set(completedDayKeySetting, serverDayKey);
                messenger.Send(new SignInDataChangedMessage(userAndUid, postSign: false));
                return false;
            }

            // If GetInfo failed (network/cookie/etc.), do not lock the whole day.
            // We still attempt sign-in, because ClaimSignInRewardAsync already has robust handling.
            bool success = await signInService.ClaimSignInRewardAsync(userAndUid, token).ConfigureAwait(false);

            if (success)
            {
                LocalSetting.Set(completedDayKeySetting, serverDayKey);
                LocalSetting.Set(lastFailureTicksKey, 0L);
            }
            else
            {
                LocalSetting.Set(lastFailureTicksKey, DateTimeOffset.UtcNow.Ticks);
            }

            messenger.Send(new SignInDataChangedMessage(userAndUid, postSign: success));
            return success;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            LocalSetting.Set(lastFailureTicksKey, DateTimeOffset.UtcNow.Ticks);
            messenger.Send(InfoBarMessage.Error(ex));
            return false;
        }
    }

    private static string GetServerDayKey(PlayerUid uid)
    {
        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForRegion(uid.Region);
        DateTimeOffset serverNow = DateTimeOffset.UtcNow.ToOffset(offset);
        return $"{uid.Region.Value}:{serverNow:yyyy-MM-dd}";
    }

    private async ValueTask<Response<SignInRewardInfo>> GetSignInInfoAsync(UserAndUid userAndUid, CancellationToken token)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            return await signInClient.GetInfoAsync(userAndUid, token).ConfigureAwait(false);
        }
    }
}
