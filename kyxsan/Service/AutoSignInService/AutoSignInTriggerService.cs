// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using kyxsan.ViewModel.User;

namespace kyxsan.Service.AutoSignIn;

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
