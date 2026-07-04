//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Service.User;
using Launcher.ViewModel.User;
using Launcher.Web.Hoyolab;
using BindingUser = Launcher.ViewModel.User.User;
using EntityUser = Launcher.Model.Entity.User;

namespace Launcher.Service.AutoSignIn;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class SignInServerDayRolloverScheduler : IRecipient<UserAndUidChangedMessage>, IDisposable
{
    private readonly IAutoSignInService autoSignInService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private CancellationTokenSource? scheduleCts;

    [GeneratedConstructor]
    public partial SignInServerDayRolloverScheduler(IServiceProvider serviceProvider);

    public void Dispose()
    {
        scheduleCts?.Cancel();
        scheduleCts?.Dispose();
        scheduleCts = null;
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is not { })
        {
            CancelSchedule();
            return;
        }

        Reschedule();
    }

    private void CancelSchedule()
    {
        scheduleCts?.Cancel();
        scheduleCts?.Dispose();
        scheduleCts = null;
    }

    private void Reschedule()
    {
        CancelSchedule();

        scheduleCts = new();
        CancellationToken token = scheduleCts.Token;

        RunAsync(token).SafeForget();
    }

    [SuppressMessage("", "SH003")]
    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            TimeSpan delay = await GetNextServerDayDelayAsync().ConfigureAwait(false);

            await Task.Delay(delay, token).ConfigureAwait(false);

            await taskContext.SwitchToBackgroundAsync();
            await autoSignInService.RunAsync(token).ConfigureAwait(false);
        }
    }

    private async ValueTask<TimeSpan> GetNextServerDayDelayAsync()
    {
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
        TimeSpan minDelay = TimeSpan.FromHours(24);

        foreach (BindingUser user in users.Source)
        {
            if (!UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
            {
                continue;
            }

            TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForRegion(userAndUid.Uid.Region);
            DateTimeOffset serverNow = DateTimeOffset.UtcNow.ToOffset(offset);
            DateTimeOffset nextServerDay = new(serverNow.Date.AddDays(1), offset);
            TimeSpan userDelay = nextServerDay - serverNow;

            if (userDelay < minDelay)
            {
                minDelay = userDelay;
            }
        }

        return minDelay < TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : minDelay;
    }
}