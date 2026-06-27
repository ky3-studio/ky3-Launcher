//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Launcher.Core.ApplicationModel;
using Launcher.Model.Entity.Database;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace Launcher.Core.Diagnostics;

[Service(ServiceLifetime.Singleton, typeof(ILauncherDiagnostics))]
internal sealed partial class LauncherDiagnostics : ILauncherDiagnostics
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial LauncherDiagnostics(IServiceProvider serviceProvider);

    public ApplicationDataContainer? LocalSettings
    {
        get
        {
            if (PackageIdentityAdapter.HasPackageIdentity)
            {
                return ApplicationData.Current.LocalSettings;
            }

            return null;
        }
    }

    public async ValueTask<int> ExecuteSqlAsync(string sql)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
        }
    }

    public ApplicationDataCompositeValue MakeApplicationDataCompositeValue(params string[] values)
    {
        ApplicationDataCompositeValue compositeValue = [];
        foreach (string value in values)
        {
            compositeValue.Add($"{CryptographicOperations.HashData(HashAlgorithmName.MD5, Encoding.UTF8.GetBytes(value)):X}", value);
        }

        return compositeValue;
    }
}