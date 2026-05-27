//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using BindingUser = kyxsan.ViewModel.User.User;
using EntityUser = kyxsan.Model.Entity.User;

namespace kyxsan.Service.User;

// For performance reason, extension method should avoid using LINQ
internal static class UserServiceExtension
{
    extension(IUserService userService)
    {
        public ValueTask<bool> RefreshCookieTokenAsync(BindingUser user)
        {
            return userService.RefreshCookieTokenAsync(user.Entity);
        }

        public async ValueTask<UserGameRole?> GetUserGameRoleByUidAsync(string uid)
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            foreach (BindingUser bindingUser in users.Source)
            {
                foreach (UserGameRole role in bindingUser.UserGameRoles.Source)
                {
                    if (role.GameUid == uid)
                    {
                        return role;
                    }
                }
            }

            return default;
        }

        public async ValueTask<BindingUser?> GetCurrentUserAsync()
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            return users.CurrentItem;
        }

        public async ValueTask<UserGameRole?> GetCurrentUserGameRoleAsync()
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            return users.CurrentItem?.UserGameRoles.CurrentItem;
        }

        public async ValueTask<string?> GetCurrentUidAsync()
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            return users.CurrentItem?.UserGameRoles?.CurrentItem?.GameUid;
        }

        public async ValueTask<UserAndUid?> GetCurrentUserAndUidAsync()
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            UserAndUid.TryFromUser(users.CurrentItem, out UserAndUid? userAndUid);
            return userAndUid;
        }

        public async ValueTask<bool> SetCurrentUserByUidAsync(string uid)
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            BindingUser? user = users.Source.SingleOrDefault(u => u.UserGameRoles.Source.Any(r => r.GameUid == uid));

            if (user is null)
            {
                return false;
            }

            await userService.TaskContext.SwitchToMainThreadAsync();
            users.MoveCurrentTo(user);

            return true;
        }

        public async ValueTask<BindingUser?> GetUserByMidAsync(string mid)
        {
            AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
            foreach (BindingUser user in users.Source)
            {
                if (user.Entity.Mid == mid)
                {
                    return user;
                }
            }

            return default;
        }
    }
}