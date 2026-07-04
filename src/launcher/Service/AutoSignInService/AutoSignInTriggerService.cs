//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.ViewModel.User;

namespace Launcher.Service.AutoSignIn;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class AutoSignInTriggerService : IRecipient<UserAndUidChangedMessage>
{
    private readonly IAutoSignInService autoSignInService;

    [GeneratedConstructor]
    public partial AutoSignInTriggerService(IServiceProvider serviceProvider);

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            autoSignInService.OnUserAndUidChangedAsync(userAndUid).AsTask().SafeForget();
        }
    }
}