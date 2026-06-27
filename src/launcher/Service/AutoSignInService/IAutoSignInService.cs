// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Launcher.ViewModel.User;

namespace Launcher.Service.AutoSignIn;

internal interface IAutoSignInService
{
    ValueTask<bool> RunAsync(CancellationToken token = default);

    ValueTask<bool> OnUserAndUidChangedAsync(UserAndUid userAndUid, CancellationToken token = default);
}