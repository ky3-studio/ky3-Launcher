//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab;
using EntityUser = kyxsan.Model.Entity.User;

namespace kyxsan.ViewModel.User;

internal sealed class UserAndUid
{
    public UserAndUid(EntityUser user, in PlayerUid role)
    {
        User = user;
        Uid = role;
    }

    public EntityUser User { get; }

    public PlayerUid Uid { get; }

    public bool IsOversea { get => User.IsOversea; }

    public static UserAndUid From(EntityUser user, PlayerUid role)
    {
        return new(user, role);
    }

    public static bool TryFromUser([NotNullWhen(true)] User? user, [NotNullWhen(true)] out UserAndUid? userAndUid)
    {
        if (user is { UserGameRoles.CurrentItem: { } role })
        {
            userAndUid = new(user.Entity, role);
            return true;
        }

        userAndUid = null;
        return false;
    }
}