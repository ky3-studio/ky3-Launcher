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
using kyxsan.Model;
using kyxsan.Model.Entity.Database;
using System.Collections.Immutable;
using System.Globalization;

namespace kyxsan.Service.Abstraction.Property;

internal sealed partial class SelectedOneBasedIndexDbProperty : DbProperty<NameValue<int>?>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly ImmutableArray<NameValue<int>> array;

    public SelectedOneBasedIndexDbProperty(IServiceProvider serviceProvider, string key, ImmutableArray<NameValue<int>> array)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.array = array;
    }

    public override NameValue<int>? Value
    {
        get
        {
            if (field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    field = string.IsNullOrEmpty(value) ? array.FirstOrDefault() : RestrictIndex(array, value);
                }
            }

            return field;
        }

        set
        {
            if (value is null)
            {
                return;
            }

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

    protected override void SetValue(NameValue<int>? value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, $"{value?.Value}"));
        }
    }

    private static NameValue<int>? RestrictIndex(ImmutableArray<NameValue<int>> array, string value)
    {
        return array.IsDefaultOrEmpty
            ? default
            : array[Math.Clamp(int.Parse(value, CultureInfo.InvariantCulture) - 1, 0, array.Length - 1)];
    }
}