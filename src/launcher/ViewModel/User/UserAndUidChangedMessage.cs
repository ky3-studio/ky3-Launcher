//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.ViewModel.User;

internal sealed class UserAndUidChangedMessage
{
    public static readonly UserAndUidChangedMessage Empty = new(null);

    public UserAndUidChangedMessage(User? user)
    {
        User = user;
        if (UserAndUid.TryFromUser(user, out UserAndUid? userAndUid))
        {
            UserAndUid = userAndUid;
        }
    }

    public User? User { get; set; }

    public UserAndUid? UserAndUid { get; }

    public static UserAndUidChangedMessage FromUser(User? user)
    {
        return new(user);
    }
}