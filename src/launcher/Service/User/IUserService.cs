//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Database;
using Launcher.Web.Hoyolab.Takumi.Binding;
using BindingUser = Launcher.ViewModel.User.User;
using EntityUser = Launcher.Model.Entity.User;

namespace Launcher.Service.User;

internal interface IUserService
{
    ITaskContext TaskContext { get; }

    ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync();

    ValueTask<ValueResult<UserOptionResultKind, string?>> ProcessInputCookieAsync(InputCookie inputCookie);

    ValueTask<bool> RefreshCookieTokenAsync(EntityUser user);

    ValueTask RemoveUserAsync(BindingUser user);

    ValueTask RefreshProfilePictureAsync(UserGameRole userGameRole);
}