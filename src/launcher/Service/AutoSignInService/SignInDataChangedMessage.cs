// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Launcher.ViewModel.User;

namespace Launcher.Service.AutoSignIn;

internal sealed class SignInDataChangedMessage
{
    public SignInDataChangedMessage(UserAndUid userAndUid, bool postSign)
    {
        UserAndUid = userAndUid;
        PostSign = postSign;
    }

    public UserAndUid UserAndUid { get; }

    public bool PostSign { get; }
}
