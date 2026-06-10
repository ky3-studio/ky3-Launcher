//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using kyxsan.Core.Database;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using System.Collections.Immutable;
using BindingUser = kyxsan.ViewModel.User.User;
using EntityUser = kyxsan.Model.Entity.User;

namespace kyxsan.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserCollectionService))]
internal sealed partial class UserCollectionService : IUserCollectionService, IDisposable
{
    private readonly IUserInitializationService userInitializationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserRepository userRepository;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly AsyncLock collectionLocker = new();

    private AdvancedDbCollectionView<BindingUser, EntityUser>? users;
    private Task? pendingBackgroundInit;

    [GeneratedConstructor]
    public partial UserCollectionService(IServiceProvider serviceProvider);

    public async ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync()
    {
        // Force run in background thread, otherwise will cause re-entrance
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        using (await collectionLocker.LockAsync().ConfigureAwait(false))
        {
            if (users is null)
            {
                ImmutableArray<EntityUser> entityUsers = userRepository.GetUserImmutableArray();

                Task<BindingUser>[] initTasks = new Task<BindingUser>[entityUsers.Length];
                for (int i = 0; i < entityUsers.Length; i++)
                {
                    initTasks[i] = userInitializationService.ResumeUserAsync(entityUsers[i]).AsTask();
                }

                BindingUser[] initializedUsers = await Task.WhenAll(initTasks).ConfigureAwait(false);

                List<BindingUser> bindingUsers = new(initializedUsers.Length);
                foreach (BindingUser user in initializedUsers)
                {
                    if (user.NeedDbUpdateAfterResume)
                    {
                        userRepository.UpdateUser(user.Entity);
                        user.NeedDbUpdateAfterResume = false;
                    }

                    bindingUsers.Add(user);
                }

                users = bindingUsers.ToAdvancedDbCollectionViewWrappedObservableReorderableDbCollection<BindingUser, EntityUser>(serviceProvider);
                users.CurrentChanged += OnCurrentUserChanged;

                await taskContext.SwitchToMainThreadAsync();
                users.MoveCurrentTo(users.Source.SelectedOrFirstOrDefault());
            }
            else if (pendingBackgroundInit is not null)
            {
                await pendingBackgroundInit.ConfigureAwait(false);
                pendingBackgroundInit = null;
            }

            return users;
        }
    }

    public async ValueTask RemoveUserAsync(BindingUser user)
    {
        ArgumentNullException.ThrowIfNull(users);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        userRepository.DeleteUserById(user.Entity.InnerId);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        users.Remove(user);

        messenger.Send(new UserRemovedMessage(user));
    }

    public async ValueTask<ValueResult<UserOptionResultKind, string?>> TryCreateAndAddUserFromInputCookieAsync(InputCookie inputCookie)
    {
        await taskContext.SwitchToBackgroundAsync();
        BindingUser? newUser = await userInitializationService.CreateUserFromInputCookieOrDefaultAsync(inputCookie).ConfigureAwait(false);

        if (newUser is null)
        {
            return new(UserOptionResultKind.CookieInvalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
        }

        if (newUser.UserGameRoles.Count is 0)
        {
            return new(UserOptionResultKind.GameRoleNotFound, SH.ServiceUserUserInfoContainsNoGameRole);
        }

        await EnsureUsersCollectionAsync().ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(users);

        await taskContext.SwitchToMainThreadAsync();
        users.Add(newUser);

        ArgumentNullException.ThrowIfNull(newUser.UserInfo);
        return new(UserOptionResultKind.Added, newUser.UserInfo.Uid);
    }

    private async ValueTask EnsureUsersCollectionAsync()
    {
        using (await collectionLocker.LockAsync().ConfigureAwait(false))
        {
            if (users is not null)
            {
                return;
            }

            ImmutableArray<EntityUser> entityUsers = userRepository.GetUserImmutableArray();
            List<BindingUser> bindingUsers = new(entityUsers.Length);

            foreach (EntityUser entity in entityUsers)
            {
                bindingUsers.Add(BindingUser.From(entity, serviceProvider));
            }

            users = bindingUsers.ToAdvancedDbCollectionViewWrappedObservableReorderableDbCollection<BindingUser, EntityUser>(serviceProvider);
            users.CurrentChanged += OnCurrentUserChanged;

            if (bindingUsers.Count > 0)
            {
                pendingBackgroundInit = ResumeExistingUsersInBackgroundAsync(bindingUsers);
            }
        }
    }

    private async Task ResumeExistingUsersInBackgroundAsync(List<BindingUser> existingUsers)
    {
        Task<BindingUser>[] tasks = new Task<BindingUser>[existingUsers.Count];
        for (int i = 0; i < existingUsers.Count; i++)
        {
            tasks[i] = userInitializationService.ResumeUserAsync(existingUsers[i]).AsTask();
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        foreach (BindingUser user in existingUsers)
        {
            if (user.NeedDbUpdateAfterResume)
            {
                userRepository.UpdateUser(user.Entity);
                user.NeedDbUpdateAfterResume = false;
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        users?.MoveCurrentTo(users.Source.SelectedOrFirstOrDefault());
    }

    public void Dispose()
    {
        if (users is not null)
        {
            users.CurrentChanged -= OnCurrentUserChanged;
        }
    }

    private void OnCurrentUserChanged(object? sender, object? args)
    {
        if (users?.CurrentItem is null)
        {
            messenger.Send(UserAndUidChangedMessage.Empty);
            return;
        }

        // Suppress the BindingUser itself to raise the message
        // This is to avoid the message being raised in the
        // BindingUser.OnCurrentUserGameRoleChanged.
        using (users.CurrentItem.SuppressCurrentUserGameRoleChangedMessage())
        {
            foreach (UserGameRole role in users.CurrentItem.UserGameRoles)
            {
                if (role.GameUid == users.CurrentItem.PreferredUid)
                {
                    users.CurrentItem.UserGameRoles.MoveCurrentTo(role);
                    break;
                }
            }

            if (users.CurrentItem.UserGameRoles.CurrentItem is null)
            {
                if (users.CurrentItem.UserGameRoles.Source.SingleOrDefault(role => role.IsChosen) is { } chosenRole)
                {
                    users.CurrentItem.UserGameRoles.MoveCurrentTo(chosenRole);
                }
                else
                {
                    users.CurrentItem.UserGameRoles.MoveCurrentToFirst();
                }
            }
        }

        messenger.Send(new UserAndUidChangedMessage(users.CurrentItem));

        // Auto sign-in is triggered by AutoSignInTriggerService via UserAndUidChangedMessage.
    }
}