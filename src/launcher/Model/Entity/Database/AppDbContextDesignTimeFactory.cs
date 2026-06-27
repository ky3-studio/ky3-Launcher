//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Design;

namespace Launcher.Model.Entity.Database;

/// <summary>
/// 此类只用于在生成迁移时提供数据库上下文
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AppDbContext CreateDbContext(string[] args)
    {
#if DEBUG
        string userdataDbName = @"D:\Launcher\Userdata.db";
        return AppDbContext.Create(default!, $"Data Source={userdataDbName}");
#else
        throw Core.ExceptionService.LauncherException.NotSupported();
#endif
    }
}
