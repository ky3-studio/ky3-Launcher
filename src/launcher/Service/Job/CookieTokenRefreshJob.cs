//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Quartz;
using kyxsan.Service.User;
using System.Collections.Immutable;

namespace kyxsan.Service.Job;

internal sealed partial class CookieTokenRefreshJob : IJob
{
    private readonly IUserService userService;
    private readonly IUserRepository userRepository;

    [GeneratedConstructor]
    public partial CookieTokenRefreshJob(IServiceProvider serviceProvider);

    [SuppressMessage("", "SH003")]
    public async Task Execute(IJobExecutionContext context)
    {
        ImmutableArray<Model.Entity.User> users = userRepository.GetUserImmutableArray();
        foreach (Model.Entity.User user in users)
        {
            if (user.SToken is null)
            {
                continue;
            }

            try
            {
                await userService.RefreshCookieTokenAsync(user).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }
}
