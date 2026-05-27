//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using kyxsan.Service.Abstraction;
using System.Collections.Immutable;

namespace kyxsan.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserRepository))]
internal sealed partial class UserRepository : IUserRepository
{
    [GeneratedConstructor]
    public partial UserRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public void DeleteUserById(Guid id)
    {
        this.DeleteByInnerId(id);
    }

    public ImmutableArray<Model.Entity.User> GetUserImmutableArray()
    {
        return this.ImmutableArray();
    }

    public void UpdateUser(Model.Entity.User user)
    {
        this.Update(user);
    }

    public void ClearUserSelection()
    {
        this.Execute(dbSet => dbSet.ExecuteUpdate(update => update.SetProperty(user => user.IsSelected, user => false)));
    }
}