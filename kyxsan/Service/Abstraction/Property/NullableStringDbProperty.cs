//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using kyxsan.Core.Database;
using kyxsan.Model.Entity.Database;

namespace kyxsan.Service.Abstraction.Property;

internal sealed partial class NullableStringDbProperty : DbProperty<string?>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;

    public NullableStringDbProperty(IServiceProvider serviceProvider, string key)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
    }

    public override string? Value
    {
        get
        {
            if (field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    field = GetValue(appDbContext, key);
                }
            }

            return field;
        }

        set
        {
            if (Volatile.Read(ref Deferring))
            {
                field = value;
                SetValue(value);
            }
            else
            {
                if (SetProperty(ref field, value))
                {
                    SetValue(value);
                }
            }
        }
    }

    protected override void SetValue(string? value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, value));
        }
    }
}