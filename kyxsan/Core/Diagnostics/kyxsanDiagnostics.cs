//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using kyxsan.Core.ApplicationModel;
using kyxsan.Model.Entity.Database;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace kyxsan.Core.Diagnostics;

[Service(ServiceLifetime.Singleton, typeof(IkyxsanDiagnostics))]
internal sealed partial class kyxsanDiagnostics : IkyxsanDiagnostics
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial kyxsanDiagnostics(IServiceProvider serviceProvider);

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