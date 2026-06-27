//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Launcher.Core.Text.Json;
using Launcher.Factory.Process;
using Launcher.Model.Entity.Database;
using Launcher.Service.ThirdPartyTool;
using Launcher.Win32;
using System.Data.Common;

namespace Launcher.Core.DependencyInjection;

internal static partial class ServiceCollectionExtension
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial IServiceCollection AddServices(this IServiceCollection services);

    extension(IServiceCollection services)
    {
        public IServiceCollection AddJsonOptions()
        {
            return services.AddSingleton(JsonOptions.Default);
        }

        public IServiceCollection AddDatabase()
        {
            return services.AddDbContextPool<AppDbContext>(AddDbContext);

            static void AddDbContext(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
            {
                string dbFile = LauncherRuntime.GetDataSubDirectoryFile("Data", "Userdata.db");
                string sqlConnectionString = $"Data Source={dbFile}";

                try
                {
                    using (AppDbContext context = AppDbContext.Create(serviceProvider, sqlConnectionString))
                    {
                        if (context.Database.GetPendingMigrations().Any())
                        {
                            serviceProvider.GetRequiredService<ILogger<AppDbContext>>().LogInformation("[Database] Performing AppDbContext Migrations");
                            context.Database.Migrate();
                        }
                    }
                }
                catch (DbException ex)
                {
                    string message = $"""
                        ky3 Launcher 在执行数据库迁移时发生错误。
                        ky3 Launcher encountered an error while performing database migration.

                        Database at '{dbFile}'

                        {ex.Message}
                        """;
                    LauncherNative.Instance.ShowErrorMessage("Warning | 警告", message);
                    ProcessFactory.KillCurrent();
                    return;
                }

                builder
#if DEBUG
                    .EnableSensitiveDataLogging()
#endif
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                    .UseSqlite(sqlConnectionString);
            }
        }

        public IServiceCollection AddThirdPartyToolService()
        {
            return services.AddSingleton<IThirdPartyToolService, ThirdPartyToolService>();
        }
    }
}