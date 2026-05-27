//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using kyxsan.Model.Entity.Database;

namespace kyxsan.Service.Abstraction;

internal static class ServiceScopeExtension
{
    extension(IServiceScope scope)
    {
        public TService GetRequiredService<TService>()
            where TService : class
        {
            return scope.ServiceProvider.GetRequiredService<TService>();
        }

        public TDbContext GetDbContext<TDbContext>()
            where TDbContext : DbContext
        {
            return scope.GetRequiredService<TDbContext>();
        }

        public AppDbContext GetAppDbContext()
        {
            return scope.GetDbContext<AppDbContext>();
        }
    }
}